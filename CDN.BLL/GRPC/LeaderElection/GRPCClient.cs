using CDN.GRPC.BsaeClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDN.BLL.GRPC.LeaderElection
{
    public class GRPCClient : AbstractGRPCClient
    {
        public CDN.GRPC.protobuf.LeadeElectionGRPC.LeadeElectionGRPCClient Client { get; private set; }
        public GRPCClient(string host, int port) : base(port, host)
        {
           
        }


        protected override void SetClient()
        {
            Client =new CDN.GRPC.protobuf.LeadeElectionGRPC.LeadeElectionGRPCClient(channel);
        }
    }
}
