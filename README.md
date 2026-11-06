# AgriStream IoT starter
Starter ASP.NET Core MVC + Azure Functions project for learning. It includes exactly four Functions: Blob trigger, HTTP GET, HTTP POST and Queue trigger.

## Azure resources
Storage Account with blob container `drone-images`, tables `SensorReadings` and `ImageAudit`, and queue `irrigation-commands`. Azure SQL Database with `Devices` table. Your student subscription previously required SQL Basic tier.

## Setup
1. Extract this folder.
2. In the root folder run:
   `dotnet new sln -n AgriStream`
   `dotnet sln add AgriStreamFunctions/AgriStreamFunctions.csproj`
   `dotnet sln add AgriStreamMVC/AgriStreamMVC.csproj`
   `dotnet restore`
3. Copy `AgriStreamFunctions/local.settings.example.json` to `local.settings.json` and replace placeholders.
4. Copy `AgriStreamMVC/appsettings.Example.json` to `appsettings.Development.json` and replace placeholders.
5. Run `Database/AgriStreamDb.sql` in Azure SQL Query editor.
6. Run the Functions project with Azure Functions Core Tools (`func start`) and MVC with `dotnet run`.

Never upload real keys, passwords, connection strings, `local.settings.json` or `appsettings.Development.json` to GitHub. The `.gitignore` excludes these local files.

The queue function updates the SQL status to `IrrigationRequested`; it does not physically operate a pump. Add actual device integration for that.
