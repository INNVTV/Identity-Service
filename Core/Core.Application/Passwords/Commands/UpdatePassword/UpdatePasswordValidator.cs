using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Commands.UpdatePassword
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordValidator()
        {

            // User Id
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Please include a user id");

            // Old Password
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Please include old password");

            // New Password
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Please include a new password");

            RuleFor(x => x.NewPassword)
                .Length(Common.Constants.Users.minPasswordLength, Common.Constants.Users.maxPasswordLength)
                .When(x => !String.IsNullOrEmpty(x.NewPassword))
                .WithMessage($"New password must be at least { Common.Constants.Users.minPasswordLength } characters in length");

            // New Password Confirm
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage("Please confirm your new password");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword)
                .When(x => !String.IsNullOrEmpty(x.ConfirmNewPassword))
                .WithMessage("Confirmation password does not match");
        }
    }
}
