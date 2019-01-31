using AutoMapper;
using IdentityService.RPC.Services;
using Serilog;
using Shared.GrpcClientLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.GrpcServer
{
    public static class ServerInitializer
    {
        public static void Initialize(int port, IServiceProvider serviceProvider, IMapper mapper)
        {
            var server = new Grpc.Core.Server
            {
                Services = {
                    IdentityRemoteServices.BindService(new IdentityServicesImplementation(serviceProvider, mapper)),
                    BackgroundRemoteServices.BindService(new BackgroundServicesImplementation())
                },
                Ports = { new Grpc.Core.ServerPort("localhost", port, Grpc.Core.ServerCredentials.Insecure) }
            };

            server.Start();

            Log.Information($"gRPC services listening on port { port }");

            //server.ShutdownAsync().Wait();
        }
    }
}
