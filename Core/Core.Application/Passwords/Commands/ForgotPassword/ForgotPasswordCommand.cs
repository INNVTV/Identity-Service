using Core.Common.Response;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<BaseResponse>
    {
        public string UserNameOrEmail { get; set; }
    }
}
