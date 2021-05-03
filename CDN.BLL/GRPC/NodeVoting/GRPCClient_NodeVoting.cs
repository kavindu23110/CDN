using CDN.GRPC.BsaeClass;
using CDN.GRPC.protobuf;

namespace CDN.BLL.GRPC.NodeVoting
{
    public class GRPCClien_NodeVoting : AbstractGRPCClient
    {

        public GRPCClien_NodeVoting(string host, int port) : base(port, host)
        {

        }

        public NodeVotingGRPC.NodeVotingGRPCClient Client { get; private set; }

        protected override void SetClient()
        {
            Client = new CDN.GRPC.protobuf.NodeVotingGRPC.NodeVotingGRPCClient(channel);
        }
    }
}
