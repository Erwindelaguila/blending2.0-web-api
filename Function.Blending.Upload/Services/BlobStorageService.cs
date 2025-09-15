using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;
using System.IO;
using Azure.Storage.Blobs.Models;
using Function.Blending.Upload.Models;


namespace Function.Blending.Upload.Services;

    public class BlobStorageService
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;
        private readonly BlobContainerClient _containerClient;
        private readonly StorageSharedKeyCredential _credentials;

        public BlobStorageService(IConfiguration configuration)
        {
            _accountName = configuration["BlobStorage:AccountName"];
            _accountKey = configuration["BlobStorage:AccountKey"];
            _containerName = configuration["BlobStorage:ContainerName"];

            var blobUri = $"https://{_accountName}.blob.core.windows.net";
            _credentials = new StorageSharedKeyCredential(_accountName, _accountKey);

            var serviceClient = new BlobServiceClient(new Uri(blobUri), _credentials);
            _containerClient = serviceClient.GetBlobContainerClient(_containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<BlobResultDto> UploadExcelAndGetLinkAsync(XLWorkbook workbook, string filePrefix )
        {
            // Nombre del archivo con timestamp
            var fileName = $"{filePrefix}-{Guid.NewGuid()}-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

            // Guardar en memoria
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            var sizeInBytes = memoryStream.Length;
            memoryStream.Position = 0;

            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(memoryStream, overwrite: true);
                
            var expiresAt = DateTimeOffset.UtcNow.AddHours(1);

            // Crear SAS Token (válido por 1 hora)
            var sasBuilder = new BlobSasBuilder 
            {
                BlobContainerName = _containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(_credentials).ToString();
            
            var downloadUrl = $"{blobClient.Uri}?{sasToken}";

            return new BlobResultDto
            {
                FileName = fileName,
                DownloadUrl = downloadUrl,
                Container = _containerName,
                ExpiresAtUtc = expiresAt,
                SizeInBytes = sizeInBytes
            };
        }
        
        public async Task<bool> DeleteFileIfExistsAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }

    }