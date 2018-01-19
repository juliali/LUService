using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.ML.Common.Utils
{
    public class BlobLoader
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=edimodel;AccountKey=SiCxHssE8KaXLyZ+539FmeiON7axn+N8rlfrZ+WyfZGEuTpSLz+KNCnVXHUISAiUcWDAq0D3XdL6tYvUU1+J5A==";
        private CloudBlobClient blobClient;

        private static BlobLoader Instance;
        public static BlobLoader GetInstance()
        {
            if (Instance == null)
            {
                Instance = new BlobLoader();
            }

            return Instance;
        }

        private BlobLoader()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            this.blobClient = storageAccount.CreateCloudBlobClient();            
        }

        public Stream ReadFileAsStream(string containerName, string blobName)
        {
            CloudBlobContainer container = this.blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            string tempPath = System.IO.Path.GetTempPath();
            string tempFilePath = tempPath +  containerName + "_" + blobName;

            if (!File.Exists(tempFilePath))
            { 
                using (var fileStream = System.IO.File.OpenWrite(tempFilePath))
                {
                
                    blockBlob.DownloadToStream(fileStream);                
                }
            }
            Stream stream = new FileStream(tempFilePath,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            return stream;
        } 
        
        public bool UploadToContainer(string containerName, string blobName, string localFilePath)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob named "blobName".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            try
            {
                // Create or overwrite the "blobName" blob with contents from a local file.
                using (var fileStream = System.IO.File.OpenRead(localFilePath))
                {
                    blockBlob.UploadFromStream(fileStream);
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool CreateContainer(string containerName)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            try
            {
                // Create the container if it doesn't already exist.
                if (container.CreateIfNotExists())
                {
                    container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }
                else
                {                   
                    Parallel.ForEach(container.ListBlobs(), x => ((CloudBlob)x).Delete());
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool DeleteContainer(string containerName)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            try
            {
                return container.DeleteIfExists();
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
