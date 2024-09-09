using Backend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Backend.Controllers;

[Route("")]
[Route("Home")]
public class HomeController : Controller
{
    private readonly SemaphoreManager _semaphoreManager;

    public HomeController(SemaphoreManager semaphoreManager)
    {
        _semaphoreManager = semaphoreManager;
    }



    [HttpGet]
    [Route("")]
    [Route("Index")]
    public IActionResult Index() => View();

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login()
    {
        var sessionId = HttpContext.Session.Id;

        if (await _semaphoreManager.TryAcquireSemaphoreAsync(sessionId))
        {
            // Store a flag in the session indicating that the semaphore is held
            HttpContext.Session.SetBool(SessionHandling.SemaphoreHeld, true);

            // Redirect to the profile page
            return RedirectToAction("index", "Profile");
        }
        else
        {
            // Redirect to the waiting page
            HttpContext.Session.SetBool(SessionHandling.WaitingForSemaphore, true);
            return RedirectToAction("index", "Waiting");
        }
    }
}
