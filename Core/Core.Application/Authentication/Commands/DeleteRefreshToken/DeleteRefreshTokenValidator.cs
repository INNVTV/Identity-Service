using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Commands.DeleteRefreshToken
{
    public class DeleteRefreshTokenValidator : AbstractValidator<DeleteRefreshTokenCommand>
    {
        public DeleteRefreshTokenValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an id");
        }
    }
}
