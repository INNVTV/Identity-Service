using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Invitations.Commands.InviteUser;
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
    }
}