using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace AgriStreamFunctions.Functions;

public class IrrigationQueueFunction
{
    private record Command(string DeviceId, string? CommandName, string? Status);

    [Function("IrrigationQueueFunction")]
    public async Task Run(
        [QueueTrigger("irrigation-commands", Connection = "StorageConnection")] string message)
    {
        var command = JsonSerializer.Deserialize<Command>(message,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (command is null || string.IsNullOrWhiteSpace(command.DeviceId))
            throw new InvalidOperationException("Invalid queue message");

        var cs = Environment.GetEnvironmentVariable("SqlConnection")
            ?? throw new InvalidOperationException("SqlConnection missing");
        await using var connection = new SqlConnection(cs);
        await connection.OpenAsync();
        const string sql = "UPDATE dbo.Devices SET OperationalStatus=@status, LastCommandAtUtc=SYSUTCDATETIME() WHERE DeviceId=@id";
        await using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@status", "IrrigationRequested");
        cmd.Parameters.AddWithValue("@id", command.DeviceId);
        if (await cmd.ExecuteNonQueryAsync() == 0)
            throw new InvalidOperationException("Device ID not found in dbo.Devices");
    }
}
