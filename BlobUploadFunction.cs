using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AgriStreamFunctions.Functions;

public class BlobUploadFunction(ILogger<BlobUploadFunction> logger)
{
    [Function("BlobUploadFunction")]
    public async Task Run(
        [BlobTrigger("drone-images/{name}", Connection = "StorageConnection")] Stream image,
        string name)
    {
        var cs = Environment.GetEnvironmentVariable("StorageConnection")
            ?? throw new InvalidOperationException("StorageConnection missing");
        var table = new TableClient(cs, "ImageAudit");
        await table.CreateIfNotExistsAsync();
        var entity = new TableEntity("DroneImages", Guid.NewGuid().ToString())
        {
            ["FileName"] = name,
            ["ProcessingStatus"] = "Uploaded",
            ["FileSizeBytes"] = image.Length,
            ["UploadDateUtc"] = DateTime.UtcNow
        };
        await table.AddEntityAsync(entity);
        logger.LogInformation("Audited image {Name}", name);
    }
}
