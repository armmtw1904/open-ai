using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using open_ai.Services;

namespace open_ai
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<OllamaService>();
                    services.AddTransient<OpenAIService>();
                })
                .Build();

            Console.WriteLine("which service to use? options- openAi; ollama; (default os ollama)");
            var ss = Console.ReadLine();
            string service = string.IsNullOrEmpty(ss) ? "ollama" : ss;
            IAIService aiService = service switch
            {
                "openAi" => host.Services.GetRequiredService<OpenAIService>(),
                _ => host.Services.GetRequiredService<OllamaService>()
            };

            Console.WriteLine("type the string content");
            string input = Console.ReadLine() ?? "";

            var resp = await aiService.ClassifyLanguagesAsync(input);
            Console.WriteLine($"response received: {System.Text.Json.JsonSerializer.Serialize(resp)}");
        }
    }
}