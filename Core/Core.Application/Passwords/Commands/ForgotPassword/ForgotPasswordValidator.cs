using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Commands.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Please specify a username or email address");
        }
    }
}
