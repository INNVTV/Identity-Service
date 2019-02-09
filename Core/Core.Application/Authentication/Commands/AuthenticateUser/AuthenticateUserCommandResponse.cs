using Core.Common.Response;
using Core.Domain.Entities;
using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Application.Authentication.Commands.AuthenticateUser
{
    [JsonObject(Title = "AuthenticationResponse")] //<-- Update name for OpenAPI/Swagger
    public class AuthenticateUserCommandResponse : BaseResponse
    {
        public AuthenticateUserCommandResponse()
            : base()
        {

        }

        public AuthenticateUserCommandResponse(IList<ValidationFailure> failures)
            : base(failures)
        {

        }

        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
    }
}