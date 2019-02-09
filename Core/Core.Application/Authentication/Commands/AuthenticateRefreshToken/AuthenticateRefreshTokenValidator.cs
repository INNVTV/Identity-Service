using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Commands.AuthenticateRefreshToken
{
    public class AuthenticateRefreshTokenValidator : AbstractValidator<AuthenticateRefreshTokenCommand>
    {
        public AuthenticateRefreshTokenValidator()
        {
            RuleFor(x => x.TokenString).NotEmpty().WithMessage("Please include the token string");
        }
    }
}
