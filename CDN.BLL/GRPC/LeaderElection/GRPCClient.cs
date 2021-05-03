using CDN.GRPC.BsaeClass;

namespace CDN.BLL.GRPC.LeaderElection
{
    public class GRPCClient_LeaderElection : AbstractGRPCClient
    {
        public CDN.GRPC.protobuf.LeadeElectionGRPC.LeadeElectionGRPCClient Client { get; private set; }
        public GRPCClient_LeaderElection(string host, int port) : base(port, host)
        {

        }


        protected override void SetClient()
        {
            Client = new CDN.GRPC.protobuf.LeadeElectionGRPC.LeadeElectionGRPCClient(channel);
        }
    }
}
