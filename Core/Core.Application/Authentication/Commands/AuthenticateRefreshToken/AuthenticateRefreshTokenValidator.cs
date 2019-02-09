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
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Please include the refresh token");
        }
    }
}
