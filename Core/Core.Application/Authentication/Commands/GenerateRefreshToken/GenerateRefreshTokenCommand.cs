using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Commands.GenerateRefreshToken
{
    public class GenerateRefreshTokenCommand : IRequest<string>
    {
        public string UserId { get; set; }
    }
}
