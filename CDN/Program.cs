using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace CDN
{
    public class Program
    {


        public static void Main(string[] args)
        {


            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                   webBuilder.UseKestrel(option => { option.Listen(IPAddress.Parse(BOD.NodeDetails.Ip), BOD.NodeDetails.Port); });
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(
                service =>
                {
                    service.AddHostedService<CDN.BLL.BackgroundServices.CommonServices>();
                    service.AddHostedService<CDN.BLL.BackgroundServices.FileWatcherService>();
                    service.AddHostedService<CDN.BLL.BackgroundServices.InitialLeaderElection>();
                    service.AddHostedService<CDN.BLL.BackgroundServices.HeartBeatService>();
                });
    }
}
