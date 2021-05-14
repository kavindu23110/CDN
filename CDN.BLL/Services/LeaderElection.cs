using CDN.BLL.GRPC.LeaderElection;
using CDN.BLL.Zookeeper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDN.BLL.Services
{
    internal class LeaderElection
    {
        private ZookeeperService zKService;

       // public GRPCClient_LeaderElection GRPCClients { get; private set; }

        public LeaderElection()
        {
            this.zKService = BLL.Statics.zk;


        }

        public async System.Threading.Tasks.Task CheckForleaderHeartBeatAsync()
        {
            CDN.GRPC.protobuf.LeaderAlive alive = null;
              
            try
            {
                var GRPCClient = new BLL.GRPC.LeaderElection.GRPCClient_LeaderElection(BOD.NodeDetails.LeaderNode, BOD.SystemPorts.LeaderElection);
                alive =await GRPCClient.Client.CheckForHeartBeatAsync(new CDN.GRPC.protobuf.LeaderAlive(), deadline: DateTime.UtcNow.AddSeconds(2));
                if (alive==null)
                {
                    ElectLeader();
                }
            }
            catch (Exception ex )
            {

                ElectLeader();
            }
            
        }

        internal void ElectLeader()
        {
            this.zKService = BLL.Statics.zk;
            var LstNodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            var LstSelected = SelectHighPriorityNodes(LstNodes);
            _ = ProceedToElectionAsync(LstSelected);
        }

        private async System.Threading.Tasks.Task ProceedToElectionAsync(List<KeyValuePair<long, string>> lstSelected)
        {
            Console.WriteLine("Leader Election Started");
            var clients = CreateGRPCClients(lstSelected);
            var response = SendLeaderElectionRequest(clients);//true=have higher nodes 
            if (response.Result)
            {
                return;
            }
            else
            {
                var electedLeader = new CDN.GRPC.protobuf.ElectedLeader() { IpAddress = BOD.NodeDetails.Ip, Priority = BOD.NodeDetails.Priority };
                BOD.NodeDetails.LeaderNode = BOD.NodeDetails.Ip;
                zKService.CreateLeaderNode(BOD.NodeDetails.LeaderNode);
                zKService.SetDataToNode($"/{BOD.NodeDetails.ClusterName}", BOD.NodeDetails.Ip);
                Console.WriteLine("Elected as Leader :" + BOD.NodeDetails.LeaderNode);
                await BroadcastElectedLeaderAsync(electedLeader);
            }
        }

        private async System.Threading.Tasks.Task BroadcastElectedLeaderAsync(CDN.GRPC.protobuf.ElectedLeader electedLeader)
        {
            var LstNodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            var clients = CreateGRPCClients(LstNodes.Where(p => p.Key < electedLeader.Priority).ToList());
            foreach (var client in clients)
            {
                _ = await client.Client.BroadcastElectedLeaderAsync(electedLeader);
                client.Stop_Channel();
            }
          
        }

        private async System.Threading.Tasks.Task<bool> SendLeaderElectionRequest(List<GRPCClient_LeaderElection> clients)
        {
            int responses = 0;

            try
            {
                foreach (var client in clients)
                {
                    var response = await client.Client.InitiateLeaderElectionAsync
                        (new CDN.GRPC.protobuf.LeaderElectionrequest() { Priority = BOD.NodeDetails.Priority }, deadline: DateTime.UtcNow.AddSeconds(5)).ResponseAsync;
                    if (response.Response)
                    {
                        responses++;
                    }
                    client.Stop_Channel();

                }

            }
            catch (Exception ex)
            {


            }
            return responses > 0;
        }

        private List<GRPCClient_LeaderElection> CreateGRPCClients(List<KeyValuePair<long, string>> lstSelected)
        {
            List<GRPCClient_LeaderElection> clients = new List<GRPCClient_LeaderElection>();
            foreach (var item in lstSelected)
            {
              
                    var client = new GRPC.LeaderElection.GRPCClient_LeaderElection(item.Value, BOD.SystemPorts.LeaderElection);
                    clients.Add(client);
            }
            return clients;
        }

        private List<KeyValuePair<long, string>> SelectHighPriorityNodes(List<KeyValuePair<long, string>> lstNodes)
        {
            return lstNodes.Where(p => p.Key > BOD.NodeDetails.Priority).ToList();

        }
    }
}