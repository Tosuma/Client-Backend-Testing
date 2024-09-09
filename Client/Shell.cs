using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Client
{
    public class Shell
    {
        private readonly HttpClient _httpClient;

        private enum MenuItem
        {
            GetInfo,
            Option2,
            Option3,
        };

        public Shell(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task Run()
        {
            string[] menuOptions = { "Get information", "Option 2", "Option 3" };
            int selectedOption = 0;
            ConsoleKeyInfo keyPress;
            do
            {
                Console.Clear();
                DisplayMenu(menuOptions, selectedOption);

                keyPress = Console.ReadKey();

                switch (keyPress.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption -= !(selectedOption - 1 < 0) ? 1 : 0;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedOption += (selectedOption + 1 < menuOptions.Length) ? 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        await ExecuteOption((MenuItem)selectedOption);
                        break;
                }
            } while (keyPress.Key != ConsoleKey.Escape);
        }

        private async Task ExecuteOption(MenuItem selectedOption)
        {
            int originalTop = Console.CursorTop;

            Console.SetCursorPosition(0, originalTop + 2);

            Console.WriteLine("Executing: " + selectedOption.ToString());

            switch (selectedOption)
            {
                case MenuItem.GetInfo:
                    string response = await MakeGetRequest();
                    Console.WriteLine("Operation returned: " + response);
                    break;
                default:
                    Console.WriteLine("Default operation encountered...");
                    break;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.SetCursorPosition(0, originalTop);
        }

        private void DisplayMenu(string[] menuOptions, int highlightedIndex)
        {
            Console.WriteLine("Select an option:");
            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == highlightedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(menuOptions[i]);

                Console.ResetColor();
            }
        }

        private async Task<string> MakeGetRequest()
        {
            //while (true)
            //{
            //    Console.WriteLine("Press 'Enter' to make a GET request. Press 'Q' to quit");

            //    var keyPress = Console.ReadKey();
            //    if (keyPress.Key == ConsoleKey.Q)
            //        break;

            //    if (keyPress.Key == ConsoleKey.Enter)
            //    {
            //        var response = await _httpClient.GetAsync("api/prompt");
            //        if (response.IsSuccessStatusCode)
            //        {
            //            string content = await response.Content.ReadAsStringAsync();
            //            //Console.WriteLine($"Backend response: {content}");
            //        }
            //        else
            //        {
            //            Console.WriteLine($"Error: {response.StatusCode}");
            //        }
            //    }
            //}

            try
            {
                var response = await _httpClient.GetAsync("api/prompt");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                else
                    return $"Error: {response.StatusCode}";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return string.Empty;
            }
            
        }
    }
}
