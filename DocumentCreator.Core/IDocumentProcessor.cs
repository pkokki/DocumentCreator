using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;

namespace DocumentCreator.Core
{
    public interface IDocumentProcessor
    {
        PagedResults<Document> GetDocuments(DocumentQuery query);
        DocumentDetails CreateDocument(string templateName, string mappingName, DocumentPayload payload);
    }
}
