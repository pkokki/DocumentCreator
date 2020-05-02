using DocumentCreator.Core.Model;
using System.Threading.Tasks;

namespace DocumentCreator.Core
{
    public interface IDocumentProcessor
    {
        PagedResults<Document> GetDocuments(DocumentQuery query);
        Task<DocumentDetails> CreateDocument(string templateName, string mappingName, DocumentPayload payload);
        DocumentDetails GetDocument(string documentId);
    }
}
