using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.RemoveFromRole
{
    [JsonObject(Title = "RemoveFromRole")] //<-- Update name for OpenAPI/Swagger
    public class RemoveFromRoleCommand : IRequest<BaseResponse>
    {
        public string Id { get; set; }
        public string RoleToRemove { get; set; }
    }
}
