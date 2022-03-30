using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcExamenTicketsApb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExamenTicketsApb.Services {
    public class ServiceBlob {
        private BlobServiceClient client;
        private string containerName;

        public ServiceBlob(BlobServiceClient client) {
            this.client = client;
            this.containerName = "tickets";
        }

        public async Task<List<BlobClass>> GetBlobsAsync() {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(this.containerName);
            List<BlobClass> blobs = new List<BlobClass>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync()) {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobClass blobClass = new BlobClass {
                    Filename = item.Name,
                    Url = blobClient.Uri.AbsoluteUri
                };
                blobs.Add(blobClass);
            }
            return blobs;
        }

        public async Task<BlobClass> FindBlobByUrl(string url) {
            List<BlobClass> blobs = await this.GetBlobsAsync();
            BlobClass blob = blobs.Where(x => x.Url == url).FirstOrDefault();
            return blob;
        }
        public async Task<BlobClass> FindBlobByName(string name) {
            List<BlobClass> blobs = await this.GetBlobsAsync();
            BlobClass blob = blobs.Where(x => x.Filename == name).FirstOrDefault();
            return blob;
        }

        public async Task DeleteBlobAsync(string url) {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(this.containerName);
            BlobClass blob = await this.FindBlobByUrl(url);
            await containerClient.DeleteBlobAsync(blob.Filename);
        }

        public async Task UploadBlobAsync(string blobName
            , Stream stream) {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(this.containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}

