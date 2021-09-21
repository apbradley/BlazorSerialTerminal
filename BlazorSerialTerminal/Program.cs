using Microsoft.AspNetCore.Hosting;
using BlazorSerialTerminal.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorSerialTerminal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    //do it this way so can be injected into page calls
                    services.AddSingleton<SerialService>();
                    services.AddHostedService(sp => sp.GetRequiredService<SerialService>());
                    services.AddSingleton<ServiceB>();
                    services.AddHostedService(sp => sp.GetRequiredService<ServiceB>());
                    //services.AddHostedService<ServiceA>();
                    //services.AddHostedService<ServiceB>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
