using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Authentication.Commands.AuthenticateUser;
using Core.Infrastructure.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using IdentityService.ServiceModels;
using Core.Application.Authentication.Models;
using Core.Application.Authentication.Commands.AuthenticateRefreshToken;

namespace IdentityService.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly ICoreConfiguration _coreConfiguration;

        public AuthenticationController(IServiceProvider serviceProvider, ICoreConfiguration coreConfiguration)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _coreConfiguration = coreConfiguration;
        }

        [Route("authenticate")]
        [HttpPost]
        public async Task<AuthenticationResponse> Post(AuthenticateUserCommand authenticateUserCommand)
        {
            var result = await _mediator.Send(authenticateUserCommand);
            return result;
        }

        [Route("authenticate/refresh")]
        [HttpPost]
        public async Task<AuthenticationResponse> Post(AuthenticateRefreshTokenCommand authenticateRefreshTokenCommand)
        {
            var result = await _mediator.Send(authenticateRefreshTokenCommand);
            return result;
        }

        [Route("public/keys")]
        [HttpGet]
        public PublicRsaKeysServiceModel Get()
        {
            var response = new PublicRsaKeysServiceModel{
                XMLString = _coreConfiguration.JSONWebTokens.PublicKeyXmlString,
                PEM = _coreConfiguration.JSONWebTokens.PublicKeyPEM
            };

            return response;
        }
    }
}