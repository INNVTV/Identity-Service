using FluentValidation;
using System;

namespace Core.Application.Users.Commands.UpdateFullName
{
    public class UpdateFullNameValidator : AbstractValidator<UpdateFullNameCommand>
    {

        public UpdateFullNameValidator()
        {

            // NewFirstName
            RuleFor(x => x.NewFirstName).NotEmpty().WithMessage("Please include a First Name");

            RuleFor(x => x.NewFirstName)
                .Length(Common.Constants.Users.minFirstNameLength, Common.Constants.Users.maxFirstNameLength)
                .When(x => !String.IsNullOrEmpty(x.NewFirstName))
                .WithMessage($"First Name must be bewtween {Common.Constants.Users.minFirstNameLength}-{Common.Constants.Users.maxFirstNameLength} characters in length");

            // NewLastName
            RuleFor(x => x.NewLastName).NotEmpty().WithMessage("Please include a Last Name");

            RuleFor(x => x.NewLastName)
                .Length(Common.Constants.Users.minLastNameLength, Common.Constants.Users.maxLastNameLength)
                .When(x => !String.IsNullOrEmpty(x.NewLastName))
                .WithMessage($"Last Name must be bewtween {Common.Constants.Users.minLastNameLength}-{Common.Constants.Users.maxLastNameLength} characters in length");
        }
    }
}
