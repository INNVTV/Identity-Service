using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.UpdateEmail
{
    [JsonObject(Title = "UpdateEmail")] //<-- Update name for OpenAPI/Swagger
    public class UpdateEmailCommand : IRequest<BaseResponse>
    {
        public string id { get; set; }
        public string NewEmail { get; set; }
    }
}
