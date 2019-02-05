using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Invitations.Commands.InviteUser;
using Core.Application.Invitations.Models;
using Core.Application.Invitations.Queries.GetExpiredInvitations;
using Core.Common.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Controllers
{
    [Route("api/invitations")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        readonly IMediator _mediator;
        //readonly IMapper _mapper; //<-- Instance version of IMapper. Used only in the Service layer for ServiceModels

        public InvitationsController(IServiceProvider serviceProvider) //, IMapper mapper)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            //_mapper = mapper;
        }

        [Route("invite")]
        [HttpPost]
        public async Task<BaseResponse> Post(InviteUserCommand inviteUserCommand)
        {
            var result = await _mediator.Send(inviteUserCommand);
            return result;
        }

        [Route("expired")]
        [HttpGet]
        public async Task<ExpiredInvitationsModel> List(int pageSize = 20, string continuationToken = null)
        {
            // We don't use the GetUserListQuery in the controller method otherwise Swagger tries to use a POST on our GET call
            var expiredInvitationsQuery = new GetExpiredInvitationsQuery { PageSize = pageSize, ContinuationToken = continuationToken };
            var result = await _mediator.Send(expiredInvitationsQuery);
            return result;

            //-----------------------------------------------------
            // TODO: DocumentDB will soon have skip/take
            // For now we use continuation token to get next batch from list
            // For even more robust query capabilities you should use the 'search' route
            //-----------------------------------------------------
        }
    }
}