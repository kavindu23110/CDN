using Grpc.Core;

namespace CDN.BLL.GRPC.LeaderElection
{
    public class GRPCServer : CDN.GRPC.BaseClass.GRPCServer
    {
        public GRPCServer(string host, int port) : base(host, port)
        {
        }
        protected override void SetService()
        {
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { CDN.GRPC.protobuf.LeadeElectionGRPC.BindService(new CDN.BLL.GRPC.LeaderElection.Service.GRPCService()) },
                Ports = { new Grpc.Core.ServerPort(host, port, ServerCredentials.Insecure) }
            };
            Server = server;
        }
    }
}
