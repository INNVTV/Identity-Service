using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Commands.UpdatePassword
{
    [JsonObject(Title = "UpdatePassword")] //<-- Update name for OpenAPI/Swagger
    public class UpdatePasswordCommand : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
