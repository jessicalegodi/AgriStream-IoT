using AgriStreamMVC.Services;
using Microsoft.AspNetCore.Mvc;
namespace AgriStreamMVC.Controllers;
public class IrrigationController(FunctionClient functions) : Controller
{
    [HttpGet] public IActionResult Index() => View();
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string deviceId)
    {
        try { ViewBag.Result = await functions.SendCommandAsync(deviceId); ViewBag.Ok = true; }
        catch (Exception ex) { ViewBag.Result = ex.Message; ViewBag.Ok = false; }
        return View();
    }
}
