using CDN.BLL.Zookeeper;
using CDN.GRPC.protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CDN.BLL.GRPC.LeaderElection.Service
{
    public class GRPCService : CDN.GRPC.protobuf.LeadeElectionGRPC.LeadeElectionGRPCBase
    {
        public override Task<Empty> BroadcastElectedLeader(ElectedLeader request, ServerCallContext context)
        {
            Console.WriteLine("Broadcasted Leader Recieved"+ request.IpAddress);
            BOD.NodeDetails.LeaderNode = request.IpAddress;
            return Task.FromResult(new Empty());
        }

        public override Task<LeaderElectionrequest> InitiateLeaderElection(LeaderElectionrequest request, ServerCallContext context)
        {
            Console.WriteLine("Broadcasted Leader Request :" + request.Priority  + "  -my Priority :" + BOD.NodeDetails.Priority);
            if (request.Priority < BOD.NodeDetails.Priority)
            {
                Task.Run(() => { new BLL.Services.LeaderElection().ElectLeader(); });
                return Task.FromResult(new LeaderElectionrequest() { Priority = BOD.NodeDetails.Priority, Response = false });
            }
            return Task.FromResult(new LeaderElectionrequest() { Priority = BOD.NodeDetails.Priority, Response = true});
        }

        public override Task<LeaderAlive> CheckForHeartBeat(LeaderAlive request, ServerCallContext context)
        {
            return base.CheckForHeartBeat(request, context);
        }
    }
}
