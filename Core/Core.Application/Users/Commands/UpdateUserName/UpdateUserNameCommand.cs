using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.UpdateUserName
{
    [JsonObject(Title = "UpdateUserName")] //<-- Update name for OpenAPI/Swagger
    public class UpdateUserNameCommand : IRequest<BaseResponse>
    {
        public string id { get; set; }
        public string NewUserName { get; set; }
    }
}
