using Azure.Storage;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PrintForMe.Helpers
{
    public static class UploadToBlob
    {
        public static BlobClient UploadFileToBlob(string strFileName, Stream fileStream)
        {
            try
            {
                var _task = Task.Run(() => UploadFileToBlobAsync(strFileName, fileStream));
                _task.Wait();
                // bool isUploaded = _task.Result;
                return _task.Result;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public static async Task<BlobClient> UploadFileToBlobAsync(string strFileName, Stream fileStream)
        {
            string AccountName = "ltechpro";
            string AccountKey = "o/nRRxxLOCrsESuNyLxlils0cG3yNZNP5sW467BR3tWOKRQMzxo3OJiR0TKGfdjYFGzdqvcDQacLxoXvl/NvHw==";
            string ImageContainer = "images";
            //string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=dsimages1055;AccountKey=Phy1Rtu0E+DDEz9C0ipHgcvJaVqpYx/0mjSw7JxsX/0tSkYNqKIFmoxV5RzCB+TO6365+dKN4r4NRhi6L4Kwtg==;EndpointSuffix=core.windows.net";


            // Create a URI to the blob
            Uri blobUri = new Uri("https://" + AccountName + ".blob.core.windows.net/" + ImageContainer + "/" + strFileName);

            // Create StorageSharedKeyCredentials object by reading
            // the values from the configuration (appsettings.json)
            StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(AccountName, AccountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            var result = await blobClient.UploadAsync(fileStream);

            return blobClient;
        }

        public static string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.Ticks.ToString();
            return strFileName;
        }
    }
}