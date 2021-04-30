using Grpc.Core;
using Grpc.Net.Client;

using System;

namespace FileShare.GPRC.GRPC.Baseclass
{
    public abstract class AbstractGRPCClient
    {
        public Channel channel;
 

        public AbstractGRPCClient(int port, string host)
        {
          
            channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
            SetClient();
      //  var x=    new protobuf.initialResponse();
           
        }

        protected  abstract void SetClient();
        

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
