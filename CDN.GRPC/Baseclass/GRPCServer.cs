using Grpc.Core;
using System;

namespace CDN.GRPC.BaseClass
{
    public abstract class GRPCServer
    {
        protected string host;
        protected int port;
        protected Grpc.Core.Server Server = null;
        public GRPCServer(string host, int port)
        {
            this.host = host;
            this.port = port;
            SetService();
            Start_server();

        }
        public Grpc.Core.Server GetServer()
        {
            return Server;
        }

        protected abstract void SetService();

        public bool Start_server()
        {
            try
            {
                Server.Start();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void Stop_server()
        {
            Server.ShutdownAsync().Wait();
        }

    }
}
