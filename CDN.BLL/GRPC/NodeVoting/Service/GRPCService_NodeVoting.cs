using CDN.BLL.Proposer;
using CDN.GRPC.protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
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

        public override Task<Empty> SendPaxosAccptanceToLeader(PaxosResponse request, ServerCallContext context)
        {
            Console.WriteLine("Leader recieved paxos response  " + DateTime.Now.ToString());
            Statics.ProposerEvent.ResponseRecieved(request);
            return Task.FromResult(new Empty());
        }


        public override Task<Empty> SendPaxosAccptanceToLearner(PaxosResponse request, ServerCallContext context)
        {
            Console.WriteLine("Learner recieved paxos response  " + DateTime.Now.ToString());
            new BLL.Leraner.Learner(true).AddResponseToLearners(request);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> AcceptAcceptanceRequest(PaxosRequest request, ServerCallContext context)
        {
            Console.WriteLine("acceptor recieved paxos response  "  + DateTime.Now.ToString());
            new BLL.Acceptor.Acceptor().SendResponseToLearnerNode(request);
            return Task.FromResult(new Empty());
        }

    }
}

