
using CDN.BLL.GRPC.NodeVoting;
using CDN.BLL.Zookeeper;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDN.BLL.Services
{
    public class VotingForNearestNode
    {
       
        private RewriteContext context;

        private ZookeeperService zKService;
 

        public VotingForNearestNode(RewriteContext context)
        {
            this.context = context;
            this.zKService = BLL.Statics.zk;
        }

        public VotingForNearestNode()
        {
            this.zKService = BLL.Statics.zk;
        }

        public async System.Threading.Tasks.Task<string> FindNearestNodeAsync(System.Net.IPAddress requestIp)
        {
            var LstNodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            var clients = CreateGRPCClients(LstNodes);
            var url = await WorkWithPaxosAlgorithm(clients);

            return url;
        }

        private async System.Threading.Tasks.Task<string> WorkWithPaxosAlgorithm(List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients)
        {
            long Id = 0;
            bool acceptance = false;
            while (acceptance == false)
            {
                 Id = DateTime.UtcNow.Ticks;
                acceptance = SendInitiateMessageAsync(Id, clients).Result;
            }
            return SendAcceptanceMessageAsync(clients, Id).Result;

        }

        private async System.Threading.Tasks.Task<string> SendAcceptanceMessageAsync(List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients, long id)
        {
            List<CDN.GRPC.protobuf.PaxosResponse> responses = new List<CDN.GRPC.protobuf.PaxosResponse>();
        
            var req = new CDN.GRPC.protobuf.PaxosRequest();
            var x = context.HttpContext.Connection.RemoteIpAddress.ToString();
            req.PID = id;
           req.ClientIp =x;
            req.FileURL = context.HttpContext.Request.Path.ToString();
            
            var deadline = DateTime.UtcNow.AddSeconds(BOD.SystemParameters.RequestDeadline);

            foreach (var item in clients)
            {
                try
                {
                    var value = await item.Client.AcceptAcceptanceRequestAsync(req, deadline: deadline);
                    responses.Add(value);
                    item.Stop_Channel();
                }
                catch (Exception ex)
                {

               
                }
            }

            return responses.Where(p => p.Distance == (responses.Min(p => p.Distance))).FirstOrDefault().FileURL;

        }

        ////public static String GetIP()
        //{
        //    //String ip =
        //    //    context.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    //if (string.IsNullOrEmpty(ip))
        //    //{
        //    //    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    //}

        //    //return ip;
        //}


        private async System.Threading.Tasks.Task<bool> SendInitiateMessageAsync(long Id, List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients)
        {
            int count = 0;
            var req = new CDN.GRPC.protobuf.Initia1PaxosRequest() { PID = Id, Success = false };
            var deadline = DateTime.UtcNow.AddSeconds(5);

            foreach (var item in clients)
            {
               
                var response = await item.Client.InitiatePaxosRequestAsync(req);
                if (response.Success)
                {
                    count++;
                }

            }
            return clients.Count == count;
        }

        private List<GRPC.NodeVoting.GRPCClien_NodeVoting> CreateGRPCClients(List<KeyValuePair<long, string>> lstNodes)
        {
            List<GRPC.NodeVoting.GRPCClien_NodeVoting> clients = new List<GRPC.NodeVoting.GRPCClien_NodeVoting>();
            foreach (var item in lstNodes)
            {
              
                    var client = new GRPC.NodeVoting.GRPCClien_NodeVoting(item.Value, BOD.SystemPorts.nearestnode);
                    clients.Add(client);
                
            }
            return clients;
        }

        public CDN.GRPC.protobuf.PaxosResponse GetResponse(CDN.GRPC.protobuf.PaxosRequest request)
        {
            var distance = new Vote.GeoDistance().calcuateIpDistance(request.ClientIp);

            var response = new CDN.GRPC.protobuf.PaxosResponse()
            {
                Distance = distance,
                FileURL = $"https://{BOD.NodeDetails.Ip}:{BOD.NodeDetails.Host}"+request.FileURL,//kts
                PID = request.PID,
            };
            return response;
            // var nodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            // var minkey = nodes.Min(p => p.Key);
            // var node = nodes.Where(p => p.Key == minkey).FirstOrDefault().Value;
            //var client= new BLL.GRPC.NodeVoting.GRPCClien_NodeVoting(node,BOD.SystemPorts.nearestnode);
            // client.Client.SendReplyToLearnerNodeAsync(response);
            // client.Stop_Channel();



        }

    }
}
