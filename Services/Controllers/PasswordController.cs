using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Passwords.Commands.ForgotPassword;
using Core.Application.Passwords.Commands.ResetPassword;
using Core.Application.Passwords.Models;
using Core.Application.Passwords.Queries.GetResetRequest;
using Core.Common.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Controllers
{
    [Route("api/password")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        readonly IMediator _mediator;

        public PasswordController(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        [Route("reset/{resetCode}")]
        [HttpGet]
        public async Task<ResetRequestModel> GetResetRequest(string resetCode)
        {
            var result = await _mediator.Send(new GetResetRequestQuery { ResetCode = resetCode });
            return result;
        }

        [Route("forgot")]
        [HttpPost]
        public async Task<BaseResponse> Forgot(string userNameOrEmail)
        {
            var result = await _mediator.Send(new ForgotPasswordCommand { UserNameOrEmail = userNameOrEmail });
            return result;
        }

        [Route("reset")]
        [HttpPost]
        public async Task<BaseResponse> Reset(ResetPasswordCommand resetPasswordCommand)
        {
            var result = await _mediator.Send(resetPasswordCommand);
            return result;
        }
    }
}