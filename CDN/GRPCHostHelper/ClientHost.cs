using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDN.GRPCHostHelper
{
    public static class ClientHost
    {
        public static void GRPCClientHost(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddGrpcClient<CDN.BLL.GRPC.LeaderElection.GRPCClient>(o=> { o.Address = new Uri(BOD.NodeDetails.Ip); });//for own
        }

        internal static void HostGRPCServer(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<CDN.BLL.GRPC.LeaderElection.Service.GRPCService>();
        }
    }
}
