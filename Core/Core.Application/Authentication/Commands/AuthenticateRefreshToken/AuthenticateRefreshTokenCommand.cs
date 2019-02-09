using Core.Application.Authentication.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Commands.AuthenticateRefreshToken
{
    public class AuthenticateRefreshTokenCommand : IRequest<AuthenticationResponse>
    {
        public string TokenString { get; set; }
    }
}
