using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.BLL.BackgroundServices
{
    public class CommonServices : BackgroundService
    {
  
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            setCurrentNodeDetails();
            CreateGRPCServers();
            HostZookeeperConnection();
           
        }

        private void HostZookeeperConnection()
        {
            BLL.Statics.zk = new CDN.BLL.Zookeeper.ZookeeperService().GetZookeeperService();
            _ = BLL.Statics.zk.CreateNodeForcurrent();
        }

        private void CreateGRPCServers()
        {
            BLL.Statics.FileShare = new BLL.GRPC.FileSystem.GRPCServer_FileSystem(BOD.NodeDetails.Ip, BOD.SystemPorts.Fileshare);
            BLL.Statics.NodeVoting = new BLL.GRPC.NodeVoting.GRPCServer_NodeVoting(BOD.NodeDetails.Ip, BOD.SystemPorts.nearestnode);
            BLL.Statics.LeaderElection = new BLL.GRPC.LeaderElection.GRPCServer(BOD.NodeDetails.Ip, BOD.SystemPorts.LeaderElection);
        }

        private static void setCurrentNodeDetails()
        {
            Guid id = Guid.NewGuid();
            BOD.NodeDetails.Ip = "127.0.0.1";
          //  BOD.NodeDetails.Ip=HttpContext.
            BOD.NodeDetails.UniqueId = id.ToString();
            BOD.NodeDetails.Priority = DateTime.UtcNow.Ticks;

        }
    }
}
