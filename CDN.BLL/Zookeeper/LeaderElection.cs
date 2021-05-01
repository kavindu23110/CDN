using CDN.BLL.GRPC.LeaderElection;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooKeeperNet;

namespace CDN.BLL.Zookeeper
{
    internal class LeaderElection
    {
        private ZookeeperService zKService;


        public LeaderElection(ZookeeperService zKService)
        {
            this.zKService = zKService;
        }

        internal void ElectLeader()
        {
            var LstNodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            var LstSelected = SelectHighPriorityNodes(LstNodes);
            _ = ProceedToElectionAsync(LstSelected);
        }

        private async System.Threading.Tasks.Task ProceedToElectionAsync(List<KeyValuePair<long, string>> lstSelected)
        {
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
                zKService.SetDataToNode($"/{BOD.NodeDetails.ClusterName}", BOD.NodeDetails.Ip);
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

        private async System.Threading.Tasks.Task<bool> SendLeaderElectionRequest(List<GRPCClient> clients)
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

        private List<GRPCClient> CreateGRPCClients(List<KeyValuePair<long, string>> lstSelected)
        {
            List<GRPCClient> clients = new List<GRPCClient>();
            foreach (var item in lstSelected)
            {
                var client = new GRPC.LeaderElection.GRPCClient(item.Value, BOD.SystemPorts.LeaderElection);
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