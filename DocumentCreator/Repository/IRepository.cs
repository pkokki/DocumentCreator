using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Repository
{
    public interface IRepository
    {
        ContentItem CreateTemplate(string templateName, byte[] contents);
        ContentItem GetLatestTemplate(string templateName);


        ContentItem GetEmptyMapping();
        ContentItem GetLatestMapping(string templateName, string mappingName);
        ContentItem CreateMapping(string templateName, string mappingName, byte[] contents);


        ContentItem CreateDocument(string templateName, string mappingName, byte[] contents);
    }
}
