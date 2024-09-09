using Backend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Backend.Controllers;

[Route("Waiting")]
public class WaitingController : Controller
{
    private readonly SemaphoreManager _semaphoreManager;

    public WaitingController(SemaphoreManager semaphoreManager)
    {
        _semaphoreManager = semaphoreManager;
    }


    [HttpGet]
    [Route("")]
    [Route("Index")]
    public IActionResult Index()
    {
        Console.WriteLine($"::: Added '{HttpContext.Session.Id}' to queue :::");
        _semaphoreManager.EnqueuSession(HttpContext.Session.Id);
        return View();
    }

    [HttpGet]
    [Route("CheckSemaphore")]
    public async Task<IActionResult> CheckSemaphore()
    {
        var sessionId = HttpContext.Session.Id;

        if (!(HttpContext.Session.GetBool(SessionHandling.WaitingForSemaphore) ?? false))
        {
            return Json(new { success = true, redirectUrl = Url.Action("index", "Home") });
        }

        
        Console.WriteLine("::: Checking for semaphore :::");

        _semaphoreManager.TryPeekQueue(out var frontOfQueue);
        if (frontOfQueue == sessionId && await _semaphoreManager.TryAcquireSemaphoreAsync(sessionId))
        {
            Console.WriteLine($"::: Removing '{sessionId}' from queue :::");
            _semaphoreManager.TryDequeue(out _);
            HttpContext.Session.SetBool(SessionHandling.SemaphoreHeld, true);
            HttpContext.Session.Remove(SessionHandling.WaitingForSemaphore);
            return Json(new {success = true, redirectUrl = Url.Action("index", "Profile")});
        }
        else
        {
            return Json(new { success = false });
        }
    }
}
