using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.Configuration;
using IdentityService.ServiceModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Controllers
{
    [Route("api/public")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly ICoreConfiguration _coreConfiguration;

        public PublicController(IServiceProvider serviceProvider, ICoreConfiguration coreConfiguration)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _coreConfiguration = coreConfiguration;
        }

        [Route("keys")]
        [HttpGet]
        public PublicRsaKeysServiceModel Get()
        {
            var response = new PublicRsaKeysServiceModel
            {
                XMLString = _coreConfiguration.JSONWebTokens.PublicKeyXmlString,
                PEM = _coreConfiguration.JSONWebTokens.PublicKeyPEM
            };

            return response;
        }
    }
}