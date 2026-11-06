using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace AgriStreamFunctions.Functions;

public class IrrigationPostFunction
{
    private record Command(string DeviceId, string CommandName = "StartIrrigation", string Status = "Pending");

    [Function("IrrigationPostFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "irrigation")] HttpRequestData req)
    {
        Command? command;
        try {
            command = await JsonSerializer.DeserializeAsync<Command>(req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        } catch (JsonException) { command = null; }

        if (command is null || string.IsNullOrWhiteSpace(command.DeviceId)) {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("A valid deviceId is required.");
            return bad;
        }

        var cs = Environment.GetEnvironmentVariable("StorageConnection")
            ?? throw new InvalidOperationException("StorageConnection missing");
        var queue = new QueueClient(cs, "irrigation-commands");
        await queue.CreateIfNotExistsAsync();
        await queue.SendMessageAsync(JsonSerializer.Serialize(command));
        var accepted = req.CreateResponse(HttpStatusCode.Accepted);
        await accepted.WriteStringAsync("Irrigation command queued.");
        return accepted;
    }
}
