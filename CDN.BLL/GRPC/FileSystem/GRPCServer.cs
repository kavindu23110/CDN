using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDN.BLL.GRPC.FileSystem
{
   public class GRPCServer_FileSystem : CDN.GRPC.BaseClass.GRPCServer
    {
        public GRPCServer_FileSystem(string host, int port) : base(host, port)
        {
        }
        protected override void SetService()
        {
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { CDN.GRPC.protobuf.FileSystemGRPC.BindService(new CDN.BLL.GRPC.FileSystem.Service.GRPCService_FileSystem()) },
                Ports = { new Grpc.Core.ServerPort(host, port, ServerCredentials.Insecure) }
            };
            Server = server;
        }
    }
}
