using Core.Common.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Commands.DeleteRefreshToken
{
    public class DeleteRefreshTokenCommand : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }
}
