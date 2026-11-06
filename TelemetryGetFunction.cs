using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace AgriStreamFunctions.Functions;

public class TelemetryGetFunction
{
    [Function("TelemetryGetFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "telemetry/{sensorId}")] HttpRequestData req,
        string sensorId)
    {
        var cs = Environment.GetEnvironmentVariable("StorageConnection")
            ?? throw new InvalidOperationException("StorageConnection missing");
        var table = new TableClient(cs, "SensorReadings");
        var filter = TableClient.CreateQueryFilter($"PartitionKey eq {sensorId}");
        var result = new List<object>();
        await foreach (var e in table.QueryAsync<TableEntity>(filter: filter))
        {
            result.Add(new {
                sensorId = e.PartitionKey, rowKey = e.RowKey,
                reading = e.ContainsKey("Reading") ? e["Reading"] : null,
                unit = e.ContainsKey("Unit") ? e["Unit"] : null,
                recordedAtUtc = e.Timestamp
            });
        }
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result.OrderByDescending(x => x.ToString()).Take(20));
        return response;
    }
}
