using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Invitations.Commands.DeleteInvitation;
using Core.Application.Invitations.Commands.InviteUser;
using Core.Application.Invitations.Models;
using Core.Application.Invitations.Queries.GetExpiredInvitations;
using Core.Application.Invitations.Queries.GetInvitationById;
using Core.Common.Response;
using Core.Domain.Entities;
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

        [Route("id/{id}")]
        [HttpGet]
        public async Task<Invitation> GetById(string id)
        {
            var result = await _mediator.Send(new GetInvitationByIdQuery { Id = id });
            return result;
        }

        [Route("expired")]
        [HttpGet]
        public async Task<ExpiredInvitationsModel> List(int pageSize = 20, string continuationToken = null)
        {
            var expiredInvitationsQuery = new GetExpiredInvitationsQuery { PageSize = pageSize, ContinuationToken = continuationToken };
            var result = await _mediator.Send(expiredInvitationsQuery);
            return result;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<BaseResponse> Delete(string id)
        {
            var deleteCommand = new DeleteInvitationCommand() { Id = id };
            return await _mediator.Send(deleteCommand);
        }
    }
}