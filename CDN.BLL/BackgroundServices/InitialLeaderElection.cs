﻿using Microsoft.Extensions.Hosting;
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
                         new CDN.BLL.Services.LeaderElection(new CDN.BLL.Zookeeper.ZookeeperService().GetZookeeperService()).ElectLeader(); ;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
} 