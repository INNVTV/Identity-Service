using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.ResetPassword
{
    [JsonObject(Title = "ResetPassword")] //<-- Update name for OpenAPI/Swagger
    public class ResetPasswordCommand : IRequest<BaseResponse>
    {
        public string id { get; set; }
        public string NewPassword { get; set; }
    }
}
