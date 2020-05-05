using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.Core.Azure
{
    public class AzuriteRepository : AzureBlobRepository
    {
        private const string AZURITE_HTTP_CONN_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";
        
        public AzuriteRepository() : base(AZURITE_HTTP_CONN_STRING)
        {
        }

        public void Clean()
        {
            blobServiceClient.GetBlobContainers()
                .Where(o => new string[] { "TEMPLATES", "MAPPINGS", "DOCUMENTS" }.Contains(o.Name))
                .ToList()
                .ForEach(c => blobServiceClient.DeleteBlobContainer(c.Name));
        }
    }
}
