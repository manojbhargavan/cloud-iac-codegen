using Azure.Storage.Blobs;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKHelper.Lib.StorageAccountPlugin
{
    public class StorageAccount
    {

        private readonly string _storageConnectionString;
        private readonly string _containerName;

        public StorageAccount()
        {
            _storageConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? "";
            if (string.IsNullOrEmpty(_storageConnectionString))
            {
                throw new Exception("AZURE_STORAGE_CONNECTION_STRING is not set for StorageAccountPlugin. Set the same or disable the plugin in Functions.inf");
            }

            _containerName = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONTAINER_NAME") ?? "";
            if (string.IsNullOrEmpty(_containerName))
            {
                throw new Exception("AZURE_STORAGE_CONTAINER_NAME is not set for StorageAccountPlugin. Set the same or disable the plugin in Functions.inf");
            }
        }

        [SKFunction, Description("Upload a files to Storage Account")]
        public async Task<string> UploadFile([Description("| Delimited full files path to upload")] string filePath)
        {
            try
            {
                string[] files = filePath.Split('|');
                StringBuilder uploadedFiles = new($"Files Uploaded to {_containerName}:\n");
                foreach (var file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);
                    var container = blobServiceClient.GetBlobContainerClient(_containerName);
                    var result = await container.UploadBlobAsync($"{fileInfo.Name}", File.OpenRead(file));
                    uploadedFiles.AppendLine($"{fileInfo.Name}");
                }
                return uploadedFiles.ToString();
            }
            catch (Exception ex)
            {
                return $"Unable to upload: {ex.Message}";
            }
        }
    }
}
