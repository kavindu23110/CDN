using CDN.BLL.Zookeeper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace CDN
{
    public class Program
    {
        public static ZookeeperService zk;

        public static void Main(string[] args)
        {
            setCurrentNodeDetails();
            zk = new CDN.BLL.Zookeeper.ZookeeperService().GetZookeeperService() ;
            zk.CreateNodeForcurrent();
            CreateHostBuilder(args).Build().Run();


        }

        private static void setCurrentNodeDetails()
        {
            Guid id = Guid.NewGuid();
            BOD.NodeDetails.Ip = "127.0.0.1";
            BOD.NodeDetails.UniqueId = id.ToString();
            BOD.NodeDetails.Priority = DateTime.UtcNow.Ticks;

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
