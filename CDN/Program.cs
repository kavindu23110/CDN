using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDN.BLL.Zookeeper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CDN
{
    public class Program
    {
        private static ZookeeperService zk;

        public static void Main(string[] args)
        {
            setCurrentNodeDetails();
            zk = new CDN.BLL.Zookeeper.ZookeeperService();
            CreateHostBuilder(args).Build().Run();
            

        }

        private static void setCurrentNodeDetails()
        {
            BOD.NodeDetails.Ip = "127.0.0.1";
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
