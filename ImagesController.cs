using AgriStreamMVC.Services;
using Microsoft.AspNetCore.Mvc;
namespace AgriStreamMVC.Controllers;
public class ImagesController(BlobUploadService uploader) : Controller
{
    [HttpGet] public IActionResult Upload() => View();
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile? image)
    {
        try { if (image is null) throw new Exception("Select an image."); ViewBag.Message = await uploader.UploadAsync(image); }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); }
        return View();
    }
}
