using DocumentCreator.Core.Azure;
using DocumentCreator.Core.Repository;
using System;
using System.Linq;
using System.Collections.Generic;
using Moq;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Models;
using Azure;
using System.Threading.Tasks;

namespace DocumentCreator
{
    public class AzureBlobRepositoryTests : IRepositoryTests
    {
        private const bool USE_AZURITE = true;
        private const string AZURITE_HTTP_CONN_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";
        //private const string AZURITE_HTTPS_CONN_STRING = "DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=https://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=https://127.0.0.1:10001/devstoreaccount1;";

        protected override IRepository CreateRepository()
        {
            BlobServiceClient blobServiceClient;
            if (USE_AZURITE && Directory.Exists(@"C:\panos\repos\DocumentCreator\DocumentCreatorAPI\temp\azurite"))
            {
                blobServiceClient = new BlobServiceClient(AZURITE_HTTP_CONN_STRING);
                blobServiceClient.GetBlobContainers()
                    .Where(o => new string[] { "TEMPLATES", "MAPPINGS", "DOCUMENTS" }.Contains(o.Name))
                    .ToList()
                    .ForEach(c => blobServiceClient.DeleteBlobContainer(c.Name));
            }
            else
            {
                blobServiceClient = MockBlobServiceClient();
            }
            return new AzureBlobRepository(blobServiceClient);
        }

        private BlobServiceClient MockBlobServiceClient()
        {
            var client = new Mock<BlobServiceClient>();
            client.Setup(c => c.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(MockBlobContainerClient());
            return client.Object;
        }

        private BlobContainerClient MockBlobContainerClient()
        {
            var client = new Mock<BlobContainerClient>();
            client.Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Returns((string name) => MockBlobClient(name));
            return client.Object;
        }

        private BlobClient MockBlobClient(string name)
        {
            var client = new Mock<BlobClient>();
            client.SetupGet(c => c.Uri).Returns(new Uri("http://mock/" + name));
            client.Setup(c => c.UploadAsync(It.IsAny<Stream>(), It.IsAny<bool>(), default))
                .Returns(MockResponseBlobContentInfo());
            return client.Object;
        }

        private Task<Response<BlobContentInfo>> MockResponseBlobContentInfo()
        {
            var response = new Mock<Response<BlobContentInfo>>();
            response.SetupGet(r => r.Value).Returns(MockBlobContentInfo());
            return Task.FromResult(response.Object);
        }

        private BlobContentInfo MockBlobContentInfo()
        {
            return BlobsModelFactory.BlobContentInfo(ETag.All, new DateTimeOffset(DateTime.Now), null, null, null, 0);
        }
    }
}
