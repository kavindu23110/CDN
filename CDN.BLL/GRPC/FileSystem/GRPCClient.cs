using CDN.GRPC.BsaeClass;
using CDN.GRPC.protobuf;

namespace CDN.BLL.GRPC.FileSystem
{
    public class GRPCClient_FileSystem : AbstractGRPCClient
    {

        public GRPCClient_FileSystem(string host, int port) : base(port, host)
        {

        }

        public FileSystemGRPC.FileSystemGRPCClient Client { get; private set; }

        protected override void SetClient()
        {
            Client = new CDN.GRPC.protobuf.FileSystemGRPC.FileSystemGRPCClient(channel);
        }
    }
}
