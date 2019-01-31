using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Roles.Commands.CreateRole;
using Core.Application.Roles.Models.Views;
using Core.Application.Roles.Queries.GetRoles;

namespace IdentityService.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper; //<-- Instance version of IMapper. Used only in the Service layer for ServiceModels

        public RolesController(IServiceProvider serviceProvider, IMapper mapper)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _mapper = mapper;
        }

        [Route("create")]
        [HttpPost]
        public async Task<CreateRoleCommandResponse> Post(string name, string description)
        {
            var createRoleCommand = new CreateRoleCommand { Name = name, Description = description };

            var result = await _mediator.Send(createRoleCommand);
            return result;
        }


        [Route("list")]
        [HttpGet]
        public async Task<RolesViewModel> List()
        {
            var getRolesQuery = new GetRolesQuery();
            var result = await _mediator.Send(getRolesQuery);
            return result;
        }
    }
}