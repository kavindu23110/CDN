using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDN.BLL.GRPC.NodeVoting
{
   public class GRPCServer_NodeVoting : CDN.GRPC.BaseClass.GRPCServer
    {
        public GRPCServer_NodeVoting(string host, int port) : base(host, port)
        {
        }
        protected override void SetService()
        {
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { CDN.GRPC.protobuf.NodeVotingGRPC.BindService(new CDN.BLL.GRPC.NodeVoting.Service.GRPCService_NodeVoting()) },
                Ports = { new Grpc.Core.ServerPort(host, port, ServerCredentials.Insecure) }
            };
            Server = server;
        }
    }
}
