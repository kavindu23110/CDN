using CDN.BLL.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.BLL.BackgroundServices
{
   public class InitialLeaderElection : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (BOD.NodeDetails.LeaderNode == null)
            {
            new CDN.BLL.Services.LeaderElection().ElectLeader();
                Console.WriteLine("Elected as Leader :" + BOD.NodeDetails.LeaderNode);
            }
            else
            {
                Console.WriteLine("select as Leader :" + BOD.NodeDetails.LeaderNode);
            }
           
            if (BOD.NodeDetails.LeaderNode != BOD.NodeDetails.Ip)
            {
                new FileSync(CDN.BOD.NodeDetails.LeaderNode).InitialFilecopyAsync();
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
} 
