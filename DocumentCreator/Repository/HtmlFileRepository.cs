using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentCreator.Repository
{
    public class HtmlFileRepository : IHtmlRepository
    {
        private readonly string baseFolder;
        private readonly string baseUrl;

        public HtmlFileRepository(string rootPath, string baseUrl)
        {
            this.baseUrl = baseUrl;
            baseFolder = Path.Combine(rootPath, "dcfs", "files", "html");
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);
        }

        public string GetUrl(string htmlName)
        {
            return $"{baseUrl}/html/{htmlName}.html";
        }

        public void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images)
        {
            if (html != null)
            {
                File.WriteAllText(Path.Combine(baseFolder, $"{htmlName}.html"), html, Encoding.UTF8);
            }
            if (images != null && images.Any())
            {
                var imageFolder = Path.Combine(baseFolder, htmlName);
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);
                foreach (var kvp in images)
                    File.WriteAllBytes(Path.Combine(imageFolder, kvp.Key), kvp.Value);
            }
        }
    }
}
