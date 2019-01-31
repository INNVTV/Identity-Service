using Core.Common.Response;
using Core.Domain.Entities;
using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Application.Roles.Commands.CreateRole
{
    [JsonObject(Title = "CreateRoleResponse")] //<-- Name for OpenAPI/Swagger
    public class CreateRoleCommandResponse : BaseResponse
    {
        public CreateRoleCommandResponse()
            : base()
        {

        }

        public CreateRoleCommandResponse(IList<ValidationFailure> failures)
            : base(failures)
        {

        }

        public Role Role { get; set; }
    }
}