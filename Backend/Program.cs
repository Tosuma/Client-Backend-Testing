using Backend.Helpers;
using Backend.Services;
using Microsoft.AspNetCore.Builder;
using System.Collections.Concurrent;

const int numOfClients = 1;
var builder = WebApplication.CreateBuilder(args);

// Register the semaphore with a maximum number of concurrent requests
builder.Services.AddSingleton(new SemaphoreManager(numOfClients));

// Register session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHostedService<SemaphoreCleanupService>();

var app = builder.Build();

// Use session middleware
app.UseSession();

// Middleware to release semaphore if session has expired
app.Use(async (context, next) =>
{
    var semaphore = context.RequestServices.GetRequiredService<SemaphoreManager>();
    var sessionSemaphoreHeld = context.Session.GetString(SessionHandling.SemaphoreHeld);

    

    if (string.IsNullOrEmpty(sessionSemaphoreHeld) && !semaphore.ClientHasSemaphore(context.Session.Id))
    {
        // If session has expired and semaphore was held, release the semaphore
        semaphore.ReleaseSemaphore(context.Session.Id);
        context.Items.Remove(SessionHandling.SemaphoreHeld);
    }

    await next.Invoke();
});


app.UseRouting();
app.UseHttpsRedirection();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // Enable CORS for all origins
app.UseAuthorization();
app.MapControllers();

app.Run();