using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.UpdateFullName
{
    [JsonObject(Title = "UpdateFullName")] //<-- Update name for OpenAPI/Swagger
    public class UpdateFullNameCommand : IRequest<BaseResponse>
    {
        public string id { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
    }
}
