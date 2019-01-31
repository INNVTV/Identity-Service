using Core.Common.Response;
using Core.Domain.Entities;
using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Application.Users.Commands.CreateUser
{
    [JsonObject(Title = "CreatUserResponse")] //<-- Update name for OpenAPI/Swagger
    public class CreateUserCommandResponse : BaseResponse
    {
        public CreateUserCommandResponse()
            : base()
        {

        }

        public CreateUserCommandResponse(IList<ValidationFailure> failures)
            : base(failures)
        {

        }

        public User User { get; set; }
    }
}