using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.BLL.BackgroundServices
{
    public class CommonServices : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            setCurrentNodeDetails();
            HostZookeeperConnection();
            CreateGRPCServers();
        }

        private void HostZookeeperConnection()
        {
            try
            {
                BLL.Statics.zk = new CDN.BLL.Zookeeper.ZookeeperService().GetZookeeperService();
                _ = BLL.Statics.zk.CreateNodeForcurrent();
            }
            catch (Exception exyh)
            {

                throw;
            }
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
            BOD.NodeDetails.UniqueId = id.ToString();
            BOD.NodeDetails.Priority = DateTime.UtcNow.Ticks;
            Console.WriteLine("Priority : " + BOD.NodeDetails.Priority.ToString());

        }
    }
}
