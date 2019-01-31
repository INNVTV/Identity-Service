using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Commands.AuthenticateUser
{
    [JsonObject(Title = "AuthenticateIdentity")] //<-- Update name for OpenAPI/Swagger
    public class AuthenticateUserCommand : IRequest<AuthenticateUserCommandResponse>
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
