using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Slottet.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Needed for WebAssembly-side DI
            builder.Services.AddScoped(_ => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7201/")
            });

            await builder.Build().RunAsync();
        }
    }
}
