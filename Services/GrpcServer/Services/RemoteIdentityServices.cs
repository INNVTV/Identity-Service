using AutoMapper;
using Grpc.Core;
using MediatR;
using RemoteServices.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Core.Application.Invitations.Commands.InviteUser;
using Core.Application.Users.Queries.GetUsersList;
using Core.Application.Users.Models.Views;

namespace IdentityService.GrpcServer.Services
{
    class RemoteIdentityServices : RemoteServices.Identity.Service.ServiceBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper; //<-- Instance version of IMapper. Used only in the Service layer for ServiceModels

        public RemoteIdentityServices(IServiceProvider serviceProvider, IMapper mapper)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _mapper = mapper;
        }

        #region Commands

        #region Invitations

        public override async Task<BaseRemoteResponse> InviteUser(InviteUserRequest request, ServerCallContext context)
        {
            Log.Information("InviteUser called via gRPC remote service {@request}", request);

            //Use AutoMapper instance to transform GrpcRequest into MediatR Request (Configured in Startup)
            var inviteUserCommand = _mapper.Map<InviteUserCommand>(request);

            var result = await _mediator.Send(inviteUserCommand);

            //Use AutoMapper instance to transform CommandResponse into GrpcResponse (Configured in Startup)
            var response = _mapper.Map<BaseRemoteResponse>(result);

            return response;

        }

        #endregion

        #endregion

        #region Queries

        public override async Task<GetUserListResponse> GetUserList(GetUserListRequest request, ServerCallContext context)
        {
            Log.Information("GetUserList called via gRPC remote service {@request}", request);

            //Use AutoMapper instance to transform GrpcRequest into MediatR Request (Configured in Startup)
            var getUserListQuery = _mapper.Map<GetUsersListQuery>(request);

            var result = await _mediator.Send(getUserListQuery);

            //Use AutoMapper instance to transform CommandResponse into GrpcResponse (Configured in Startup)
            var response = _mapper.Map<GetUserListResponse>(result);

            return response;

        }

        #endregion

    }
}
