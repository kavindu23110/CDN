using Grpc.Core;

namespace CDN.GRPC.BaseClass
{
    public abstract class AbstractGRPCClient
    {
        public Channel channel;


        public AbstractGRPCClient(int port, string host)
        {
            channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
            SetClient();
        }

        protected abstract void SetClient();


        public Grpc.Core.Channel GetChannel()
        {
            return channel;
        }

        public void Stop_Channel()
        {
            channel.ShutdownAsync().Wait();
        }
    }
}
