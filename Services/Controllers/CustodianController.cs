using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Custodian.Commands;
using Core.Application.Custodian.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Controllers
{
    [Route("api/custodian")]
    [ApiController]
    public class CustodianController : ControllerBase
    {
        readonly IMediator _mediator;

        public CustodianController(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        [Route("run")]
        [HttpGet]
        public async Task<CustodialReport> Run()
        {
            var runCustodialTasksCommand = new RunCustodialTasksCommand();
            var result = await _mediator.Send(runCustodialTasksCommand);

            return result;
        }
    }
}