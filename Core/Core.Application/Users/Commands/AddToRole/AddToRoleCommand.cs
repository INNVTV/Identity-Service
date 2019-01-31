using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.AddToRole
{
    [JsonObject(Title = "AddToRole")] //<-- Update name for OpenAPI/Swagger
    public class AddToRoleCommand : IRequest<BaseResponse>
    {
        public string Id { get; set; }
        public string RoleToAdd { get; set; }
    }
}
