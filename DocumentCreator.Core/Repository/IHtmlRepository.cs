using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public interface IHtmlRepository
    {
        string GetUrl(string htmlName);
        void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images);
    }
}
