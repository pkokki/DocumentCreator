using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-3.1
        // HttpClient is intended to be instantiated once per application, rather than per-use.
        private static readonly HttpClient httpClient = new HttpClient();

        public ExcelValue ENCODEURL(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string url)) return ExcelValue.NA;

            return new ExcelValue.TextValue(Uri.EscapeDataString(url), scope.OutLanguage);
        }

        public ExcelValue WEBSERVICE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string url)) return ExcelValue.NA;

            try
            {
                var response = httpClient.GetStringAsync(url).GetAwaiter().GetResult();
                return new ExcelValue.TextValue(response, scope.OutLanguage);
            }
            catch
            {
                return ExcelValue.VALUE;
            }
        }

        public ExcelValue FILTERXML(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string xmlText)) return ExcelValue.NA;
            if (args.NotText(1, null, scope.OutLanguage, out string xpath)) return ExcelValue.NA;

            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlText);
                var xmlNode = xml.SelectSingleNode(xpath);
                return new ExcelValue.TextValue(xmlNode.InnerText, scope.OutLanguage);
            }
            catch
            {
                return ExcelValue.VALUE;
            }
        }
    }
}
