using AgriStreamMVC.Services;
using Microsoft.AspNetCore.Mvc;
namespace AgriStreamMVC.Controllers;
public class DashboardController(FunctionClient functions) : Controller
{
    public async Task<IActionResult> Index()
    {
        try { ViewBag.Telemetry = await functions.GetTelemetryAsync(); }
        catch (Exception ex) { ViewBag.Telemetry = ex.Message; }
        return View();
    }
}
