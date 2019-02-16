using FluentValidation;
using System;

namespace Core.Application.Users.Commands.UpdateFullName
{
    public class UpdateFullNameValidator : AbstractValidator<UpdateFullNameCommand>
    {

        public UpdateFullNameValidator()
        {

            // NewFirstName
            RuleFor(x => x.NewFirstName).NotEmpty().WithMessage("Please include a First name");

            RuleFor(x => x.NewFirstName)
                .Length(Common.Constants.Users.minFirstNameLength, Common.Constants.Users.maxFirstNameLength)
                .When(x => !String.IsNullOrEmpty(x.NewFirstName))
                .WithMessage($"First name must be bewtween {Common.Constants.Users.minFirstNameLength}-{Common.Constants.Users.maxFirstNameLength} characters in length");

            RuleFor(x => x.NewFirstName)
                .Must(Common.Validation.Methods.NotIncludeNumbersSpacesOrSpecialCharacters)
                .When(x => !String.IsNullOrEmpty(x.NewFirstName))
                .WithMessage(x => "First name cannot include numbers, spaces or special characters");


            // NewLastName
            RuleFor(x => x.NewLastName).NotEmpty().WithMessage("Please include a last name");

            RuleFor(x => x.NewLastName)
                .Length(Common.Constants.Users.minLastNameLength, Common.Constants.Users.maxLastNameLength)
                .When(x => !String.IsNullOrEmpty(x.NewLastName))
                .WithMessage($"Last name must be bewtween {Common.Constants.Users.minLastNameLength}-{Common.Constants.Users.maxLastNameLength} characters in length");

            RuleFor(x => x.NewLastName)
                .Must(Common.Validation.Methods.NotIncludeNumbersSpacesOrSpecialCharacters)
                .When(x => !String.IsNullOrEmpty(x.NewLastName))
                .WithMessage(x => "Last name cannot include numbers, spaces or special characters");
        }
    }
}
