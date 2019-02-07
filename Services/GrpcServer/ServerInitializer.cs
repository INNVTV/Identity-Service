using AutoMapper;
using IdentityService.GrpcServer.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.GrpcServer
{
    public static class ServerInitializer
    {
        public static void Initialize(string host, int port, IServiceProvider serviceProvider, IMapper mapper)
        {
            var server = new Grpc.Core.Server
            {
                Services = {
                    RemoteServices.Identity.Service.BindService (new RemoteIdentityServices(serviceProvider, mapper))
                },
                Ports = { new Grpc.Core.ServerPort(host, port, Grpc.Core.ServerCredentials.Insecure) }
            };

            server.Start();

            Log.Information($"gRPC services listening on port { port }");

            //server.ShutdownAsync().Wait();
        }
    }
}
