using Backend.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("Profile")]
public class ProfileController : Controller
{
    private readonly SemaphoreManager _semaphoreManager;

    public ProfileController(SemaphoreManager semaphoreManager)
    {
        _semaphoreManager = semaphoreManager;
    }

    [HttpGet]
    [Route("")]
    [Route("Index")]
    public IActionResult Index()
    {
        if (!_semaphoreManager.ClientHasSemaphore(HttpContext.Session.Id))
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [Route("Logout")]
    public IActionResult Logout()
    {
        // Check if the semaphore is held
        var semaphoreHeld = HttpContext.Session.GetBool(Helpers.SessionHandling.SemaphoreHeld) ?? false;

        if (semaphoreHeld)
        {
            _semaphoreManager.ReleaseSemaphore(HttpContext.Session.Id);
            HttpContext.Session.Remove(Helpers.SessionHandling.SemaphoreHeld);
        }

        // Redirect to the home page
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Edit")]
    public IActionResult Edit()
    {
        var semaphoreHeld = HttpContext.Session.GetBool(Helpers.SessionHandling.SemaphoreHeld) ?? false;

        if (!semaphoreHeld)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [Route("SaveAndExit")]
    public IActionResult SaveAndExit()
    {
        return Redirect("Index");
    }
}