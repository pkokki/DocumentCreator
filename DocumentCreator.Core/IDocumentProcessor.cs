using DocumentCreator.Core.Model;
using System.Threading.Tasks;

namespace DocumentCreator.Core
{
    public interface IDocumentProcessor
    {
        PagedResults<Document> GetDocuments(DocumentQuery query);
        DocumentDetails GetDocument(string documentId);
        Task<DocumentDetails> CreateDocument(string templateName, string mappingName, DocumentPayload payload);
    }
}
