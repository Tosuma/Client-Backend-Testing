using Client;
using System.Reflection.Emit;

class Program
{

    static async Task Main(string[] args)
    {
        var baseAddress = new Uri("https://localhost:5001/");
        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            Shell shell = new Shell(httpClient);
            await shell.Run();
        }
        Console.ReadLine(); // Keep console open
    }
}