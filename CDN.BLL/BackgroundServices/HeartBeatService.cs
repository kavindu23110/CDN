using CDN.BLL.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.BLL.BackgroundServices
{
    public class HeartBeatService : BackgroundService
    {
        public HeartBeatService()
        {
            LElection = new LeaderElection();
        }
        private System.Threading.Timer _timer;


        internal LeaderElection LElection { get; private set; }

        [Obsolete]
        protected override Task ExecuteAsync(CancellationToken stoppingToken)

        {


            TimeSpan interval = TimeSpan.FromSeconds(2);
            Action action = () =>
            {

                _timer = new System.Threading.Timer(
                CheckforLeader,
                    null,
                    TimeSpan.Zero,
                    interval
                );

            };
            Task.Run(action);
            return Task.CompletedTask;
        }


        private void CheckforLeader(object state)
        {
            if (BOD.NodeDetails.LeaderNode != BOD.NodeDetails.Ip)
            {
                _ = LElection.CheckForleaderHeartBeatAsync();

            }
        }
    }
}
