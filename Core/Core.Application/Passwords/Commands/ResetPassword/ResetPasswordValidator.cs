using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Commands.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            // Reset Code
            RuleFor(x => x.ResetCode).NotEmpty().WithMessage("Please include a reset code");

            // User Id
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Please include a user id");

            // Password
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Please include a password");

            RuleFor(x => x.NewPassword)
                .Length(Common.Constants.Users.minPasswordLength, Common.Constants.Users.maxPasswordLength)
                .When(x => !String.IsNullOrEmpty(x.NewPassword))
                .WithMessage($"Password must be at least { Common.Constants.Users.minPasswordLength } characters in length");

            // Password Confirm
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage("Please confirm your password");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword)
                .When(x => !String.IsNullOrEmpty(x.ConfirmNewPassword))
                .WithMessage("Confirmation password does not match");
        }
    }
}
