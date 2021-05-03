using CDN.BLL.Zookeeper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CDN
{
    public class Program
    {
   

        public static void Main(string[] args)
        {
            

            CreateHostBuilder(args).Build().Run();
        }

 

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(
                service => {
                    service.AddHostedService<CDN.BLL.BackgroundServices.CommonServices>();
                    service.AddHostedService<CDN.BLL.BackgroundServices.FileWatcherService>();
                    service.AddHostedService<CDN.BLL.BackgroundServices.InitialLeaderElection>();
                });
    }
}
