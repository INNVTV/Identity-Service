using AutoMapper;
using Grpc.Core;
using MediatR;
using Serilog;
using Shared.GrpcClientLibrary;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Core.Common.Response;
using Core.Application.Users.Commands.CreateUser;

namespace IdentityService.RPC.Services
{
    class IdentityServicesImplementation : Shared.GrpcClientLibrary.IdentityRemoteServices.IdentityRemoteServicesBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper; //<-- Instance version of IMapper. Used only in the Service layer for ServiceModels

        public IdentityServicesImplementation(IServiceProvider serviceProvider, IMapper mapper)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _mapper = mapper;
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest createUserRequest, ServerCallContext context)
        {
            Log.Information("CreateUser called via gRPC remote service {@createUserRequest}", createUserRequest);

            //Use AutoMapper instance to transform GrpcRequest into MediatR Request (Configured in Startup)
            var createUserCommand = _mapper.Map<CreateUserCommand>(createUserRequest);

            var result = await _mediator.Send(createUserCommand);

            //Use AutoMapper instance to transform CommandResponse into GrpcResponse (Configured in Startup)
            CreateUserResponse createUserResponse = _mapper.Map<CreateUserResponse>(result);

            return createUserResponse;
        }
    }
}
