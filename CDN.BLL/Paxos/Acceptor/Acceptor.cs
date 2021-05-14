using CDN.BLL.Zookeeper;
using System;
using System.Linq;

namespace CDN.BLL.Acceptor
{
    public class Acceptor
    {



        private ZookeeperService zKService;



        public Acceptor()
        {
            this.zKService = BLL.Statics.zk;
        }

        public void SendResponseToLearnerNode(CDN.GRPC.protobuf.PaxosRequest request)
        {
            var LstNodes = zKService.GetClusterNodes(BOD.NodeDetails.ClusterName);
            var MinimumId = LstNodes.Min(p => p.Key);
            var node = LstNodes.Where(p => p.Key == MinimumId).Select(p => p.Value).FirstOrDefault();
            Console.WriteLine("Send  paxos response to learner " +node +"  "+ DateTime.Now.ToString());
            var client = new GRPC.NodeVoting.GRPCClien_NodeVoting(node, BOD.SystemPorts.nearestnode);
            client.Client.SendPaxosAccptanceToLearner(NodeResponse(request));
            client.Stop_Channel();
        }
        public CDN.GRPC.protobuf.PaxosResponse NodeResponse(CDN.GRPC.protobuf.PaxosRequest request)
        {
            var distance = new Vote.GeoDistance().calcuateIpDistance(request.ClientIp);

            var response = new CDN.GRPC.protobuf.PaxosResponse()
            {
                Distance = distance,
                FileURL = $"http://{BOD.NodeDetails.Ip}:{BOD.NodeDetails.Port}" + request.FileURL,//kts
                PID = request.PID,
            };
            return response;
        }

    }
}
