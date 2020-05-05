using Azure.Storage.Blobs;
using DocumentCreator.Core.Azure;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    [Trait("Category", "LocalOnly")]
    public class AzureBlobRepositoryTests : IRepositoryTests
    {
        protected override IRepository CreateRepository()
        {
            var repository = new AzuriteRepository();
            repository.Clean();
            return repository;
        }

        protected override string TemplateNamePattern => "[A-Za-z0-9]+";
        protected override string TemplateFilePattern => "[A-Za-z0-9]+.docx";

        protected override string MappingNamePattern => "[A-Za-z0-9]+_[0-9]+_[A-Za-z0-9]+";
        protected override string MappingFilePattern => "[A-Za-z0-9]+_[0-9]+_[A-Za-z0-9]+.xlsm";

        protected override string DocumentNamePattern => "[A-Za-z0-9]+";
        protected override string DocumentFilePattern => "[A-Za-z0-9]+.docx";

        
        //private readonly Dictionary<Tuple<string, string>, Tuple<BlobItem, BlobProperties, Stream>> mockItems
        //    = new Dictionary<Tuple<string, string>, Tuple<BlobItem, BlobProperties, Stream>>();
        //private readonly Dictionary<Tuple<string, string>, List<Tuple<BlobItem, BlobProperties, Stream>>> mockSnapshots 
        //    = new Dictionary<Tuple<string, string>, List<Tuple<BlobItem, BlobProperties, Stream>>>();

        //private BlobServiceClient MockBlobServiceClient()
        //{
        //    var client = new Mock<BlobServiceClient>();
        //    client.Setup(c => c.GetBlobContainerClient(It.IsAny<string>()))
        //        .Returns((string containerName) => MockBlobContainerClient(containerName));
        //    return client.Object;
        //}

        //private BlobContainerClient MockBlobContainerClient(string containerName)
        //{
        //    var client = new Mock<BlobContainerClient>();
        //    client.Setup(c => c.Name).Returns(containerName);
        //    client.Setup(c => c.Uri).Returns(new Uri($"http://mock/{containerName}"));
        //    client.Setup(c => c.GetBlobClient(It.IsAny<string>()))
        //        .Returns((string blobName) => MockBlobClient(containerName, blobName));


        //    Pageable<BlobItem> pageableItems = null;
        //    client.Setup(c => c.GetBlobs(BlobTraits.Metadata, It.IsAny<BlobStates>(), It.IsAny<string>(), default))
        //        .Callback((BlobTraits traits, BlobStates states, string prefix, CancellationToken cancellationToken) =>
        //            pageableItems = CallbackMockPageableBlobItem(containerName, traits, states, prefix)
        //            )
        //        .Returns(() => pageableItems);

        //    return client.Object;
        //}

        //private Pageable<BlobItem> CallbackMockPageableBlobItem(string containerName, BlobTraits traits, BlobStates states, string prefix)
        //{
        //    IEnumerator<BlobItem> items;
        //    if (states == BlobStates.Snapshots && prefix != null)
        //    {
        //        items = mockSnapshots
        //            .Where(o => o.Key.Item1 == containerName && o.Key.Item2.StartsWith(prefix))
        //            .SelectMany(o => o.Value)
        //            .Select(o => o.Item1)
        //            .Union(mockItems
        //                .Where(o => o.Key.Item1 == containerName && o.Key.Item2.StartsWith(prefix))
        //                .Select(o => o.Value.Item1))
        //            .GetEnumerator();
        //    }
        //    else
        //    {
        //        items = mockItems
        //            .Where(o => o.Key.Item1 == containerName)
        //            .Select(o => o.Value.Item1)
        //            .GetEnumerator();
        //    }
        //    var pageableMock = new Mock<Pageable<BlobItem>>();
        //    pageableMock.Setup(p => p.GetEnumerator())
        //        .Returns(items);
        //    return pageableMock.Object;
        //}

        //private BlobClient MockBlobClient(string containerName, string blobName)
        //{
        //    var client = new Mock<BlobClient>();
        //    client.Setup(c => c.Name).Returns(blobName);
        //    client.SetupGet(c => c.Uri).Returns(new Uri($"http://mock/{containerName}/{blobName}"));
        //    client.Setup(c => c.UploadAsync(It.IsAny<Stream>(), null, It.IsAny<IDictionary<string, string>>(), null, null, null, default, default))
        //        .Returns((Stream content, BlobHttpHeaders httpHeaders, IDictionary<string, string> metadata, BlobRequestConditions conditions, IProgress<long> progressHandler, AccessTier? accessTier, StorageTransferOptions transferOptions, CancellationToken cancellationToken)
        //            => MockUploadResponse(containerName, blobName, metadata, content));
        //    client.Setup(c => c.CreateSnapshotAsync(It.IsAny<IDictionary<string, string>>(), null, default))
        //        .Callback((IDictionary<string, string> metadata, BlobRequestConditions conditions, CancellationToken cancellationToken) => {
        //            var key = Tuple.Create(containerName, blobName);
        //            var content = mockItems[key].Item3;
        //            StoreSnapshot(containerName, blobName, metadata, content);
        //        });

        //    Response<BlobProperties> responseBlobProperties = null;
        //    client.Setup(c => c.GetPropertiesAsync(null, default))
        //        .Callback(() => responseBlobProperties = CallbackMockResponseBlobProperties(containerName, blobName))
        //        .Returns(() => Task.FromResult(responseBlobProperties));

        //    Response<BlobDownloadInfo> responseBlobDownloadInfo = null;
        //    client.Setup(c => c.Download())
        //        .Callback(() => responseBlobDownloadInfo = CallbackMockDownloadResponse(containerName, blobName))
        //        .Returns(() => responseBlobDownloadInfo);

        //    return client.Object;
        //}

        //private Response<BlobDownloadInfo> CallbackMockDownloadResponse(string containerName, string blobName)
        //{
        //    var value = CallbackMockBlobDownloadInfo(containerName, blobName);
        //    if (value != null)
        //    {
        //        var response = new Mock<Response<BlobDownloadInfo>>();
        //        response
        //            .SetupGet(r => r.Value).Returns(value);
        //        return response.Object;
        //    }
        //    return null;
        //}

        //private BlobDownloadInfo CallbackMockBlobDownloadInfo(string containerName, string blobName)
        //{
        //    var key = Tuple.Create(containerName, blobName);
        //    if (mockItems.TryGetValue(key, out Tuple<BlobItem, BlobProperties, Stream> item))
        //    {
        //        var stream = item.Item3;
        //        return BlobsModelFactory.BlobDownloadInfo(
        //            content: stream,
        //            metadata: item.Item1.Metadata,
        //            contentLength: stream.Length,
        //            lastModified: DateTime.Now
        //            );
        //    }
        //    return null;
        //}

        //private void StoreSnapshot(string containerName, string blobName, IDictionary<string, string> metadata, Stream stream)
        //{
        //    var key = Tuple.Create(containerName, blobName);
        //    if (!mockSnapshots.TryGetValue(key, out List<Tuple<BlobItem, BlobProperties, Stream>> list))
        //    {
        //        list = new List<Tuple<BlobItem, BlobProperties, Stream>>();
        //        mockSnapshots[key] = list;
        //    }
        //    list.Add(Tuple.Create(
        //        BlobsModelFactory.BlobItem(blobName, false, MockBlobItemProperties(containerName, blobName, metadata, stream), null, metadata),
        //        MockBlobProperties(metadata),
        //        stream
        //        ));
        //}
        //private Task<Response<BlobContentInfo>> MockUploadResponse(string containerName, string blobName, IDictionary<string, string> metadata, Stream stream)
        //{
        //    var key = Tuple.Create(containerName, blobName);
        //    mockItems[key] = Tuple.Create(
        //        BlobsModelFactory.BlobItem(blobName, false, MockBlobItemProperties(containerName, blobName, metadata, stream), null, metadata),
        //        MockBlobProperties(metadata),
        //        stream
        //        );

        //    var response = new Mock<Response<BlobContentInfo>>();
        //    response.SetupGet(r => r.Value)
        //        .Returns(() => BlobsModelFactory.BlobContentInfo(ETag.All, new DateTimeOffset(DateTime.Now), null, null, null, 0));
        //    return Task.FromResult(response.Object);
        //}

        //private Response<BlobProperties> CallbackMockResponseBlobProperties(string containerName, string blobName)
        //{
        //    if (mockItems.TryGetValue(Tuple.Create(containerName, blobName), out Tuple<BlobItem, BlobProperties, Stream> item))
        //    {
        //        var response = new Mock<Response<BlobProperties>>();
        //        response.SetupGet(r => r.Value).Returns(item.Item2);
        //        return response.Object;
        //    }
        //    return null;
        //}

        //private BlobItemProperties MockBlobItemProperties(string containerName, string blobName, IDictionary<string, string> metadata, Stream stream)
        //{
        //    return BlobsModelFactory.BlobItemProperties(true, 
        //        contentLength: stream.Length, 
        //        lastModified: new DateTimeOffset(DateTime.Now)
        //        );
        //}

        //private BlobProperties MockBlobProperties(IDictionary<string, string> metadata)
        //{
        //    return BlobsModelFactory.BlobProperties(
        //        new DateTimeOffset(DateTime.Now),
        //        default, default, 12345, default, ETag.All,
        //        null, null, null, null, null, null, false, 0L, default, null, null, 0, null, false,
        //        null, null, null, null, new DateTimeOffset(DateTime.Now), null, default, false, metadata, null,
        //        new DateTimeOffset(DateTime.Now), new DateTimeOffset(DateTime.Now), null
        //        );
        //}


    }
}
