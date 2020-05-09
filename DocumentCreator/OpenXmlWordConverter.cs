using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DocumentCreator
{
    public class OpenXmlWordConverter
    {

        public class HtmlConversion
        {
            public string Html { get; set; }
            public Dictionary<string, byte[]> Images { get; } = new Dictionary<string, byte[]>();
        }

        public static HtmlConversion ConvertToHtml(Stream ms, string pageTitle, string documentName = null)
        {
            using var wDoc = WordprocessingDocument.Open(ms, true);
            return ConvertToHtml(wDoc, pageTitle, documentName);
        }

        // https://github.com/OfficeDev/Open-Xml-PowerTools/blob/2f9134bd5abe0547fcf3d803b40b1401d6e58020/OpenXmlPowerToolsExamples/HtmlConverter01/HtmlConverter01.cs
        private static HtmlConversion ConvertToHtml(WordprocessingDocument wDoc, string pageTitle, string documentName = null)
        {
            var htmlConversion = new HtmlConversion();
            int imageCounter = 0;
            var part = wDoc.CoreFilePropertiesPart;
            if (part != null)
            {
                pageTitle = (string)part.GetXDocument().Descendants(DC.title).FirstOrDefault() ?? pageTitle;
            }
            // TODO: Determine max-width from size of content area.
            var settings = new WmlToHtmlConverterSettings()
            {
                AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                PageTitle = pageTitle,
                FabricateCssClasses = true,
                CssClassPrefix = "pt-",
                RestrictToSupportedLanguages = false,
                RestrictToSupportedNumberingFormats = false,
                ImageHandler = imageInfo =>
                {
                    ++imageCounter;
                    string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                    ImageFormat imageFormat = null;
                    if (extension == "png")
                        imageFormat = ImageFormat.Png;
                    else if (extension == "gif")
                        imageFormat = ImageFormat.Gif;
                    else if (extension == "bmp")
                        imageFormat = ImageFormat.Bmp;
                    else if (extension == "jpeg")
                        imageFormat = ImageFormat.Jpeg;
                    else if (extension == "tiff")
                    {
                        // Convert tiff to gif.
                        extension = "gif";
                        imageFormat = ImageFormat.Gif;
                    }
                    else if (extension == "x-wmf")
                    {
                        extension = "wmf";
                        imageFormat = ImageFormat.Wmf;
                    }

                    // If the image format isn't one that we expect, ignore it,
                    // and don't return markup for the link.
                    if (imageFormat == null)
                        return null;

                    // Return image buffers only when template is converted
                    string imageFilename = $"image{imageCounter}.{extension}";
                    string imageUrl = $"./{pageTitle}/{imageFilename}";
                    if (documentName == null)
                    {
                        try
                        {
                            using var ms = new MemoryStream();
                            imageInfo.Bitmap.Save(ms, imageFormat);
                            htmlConversion.Images.Add(imageFilename, ms.ToArray());
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            return null;
                        }
                    }

                    XElement img = new XElement(Xhtml.img,
                        new XAttribute(NoNamespace.src, imageUrl),
                        imageInfo.ImgStyleAttribute,
                        imageInfo.AltText != null ?
                            new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                    return img;
                }
            };

            XElement htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);

            // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
            // we are using HTML5.
            var html = new XDocument(
                new XDocumentType("html", null, null, null),
                htmlElement);

            // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
            // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
            // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
            // for detailed explanation.
            //
            // If you further transform the XML tree returned by ConvertToHtmlTransform, you
            // must do it correctly, or entities will not be serialized properly.

            var htmlString = html.ToString(SaveOptions.DisableFormatting);
            htmlConversion.Html = htmlString;
            return htmlConversion;
        }
    }
}
