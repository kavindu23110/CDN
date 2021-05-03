using CDN.GRPC.protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Threading.Tasks;

namespace CDN.BLL.GRPC.NodeVoting.Service
{
    public class GRPCService_NodeVoting : CDN.GRPC.protobuf.NodeVotingGRPC.NodeVotingGRPCBase
    {
        public override Task<Initia1PaxosRequest> InitiatePaxosRequest(Initia1PaxosRequest request, ServerCallContext context)
        {
            if (request.PID > BOD.NodeDetails.PaxosPriority)
            {
                BOD.NodeDetails.PaxosPriority = request.PID;
                request.Success = true;
                return Task.FromResult(request);
            }
            Task.Delay(10);
            return Task.FromResult(request);
        }

        public override Task<PaxosResponse> AcceptAcceptanceRequest(PaxosRequest request, ServerCallContext context)
        {
            var response = new BLL.Services.VotingForNearestNode().GetResponse(request);
            return Task.FromResult(response);
        }


    }
}

