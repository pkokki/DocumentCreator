using DocumentCreator.Core.Model;

namespace DocumentCreator.Core
{
    public interface IDocumentProcessor
    {
        PagedResults<Document> GetDocuments(DocumentQuery query);
        DocumentDetails CreateDocument(string templateName, string mappingName, DocumentPayload payload);
        DocumentDetails GetDocument(string documentId);
    }
}
