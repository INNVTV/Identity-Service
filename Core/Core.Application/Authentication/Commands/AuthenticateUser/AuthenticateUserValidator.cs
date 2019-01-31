using FluentValidation;
using MediatR;
using System;

namespace Core.Application.Authentication.Commands.AuthenticateUser
{
    public class AuthenticateUserValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AuthenticateUserValidator()
        {

            // UserNameOrEmail
            RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Please include a UserName or an Email");

            RuleFor(x => x.UserNameOrEmail)
                .Length(1, 240)
                .When(x => !String.IsNullOrEmpty(x.UserNameOrEmail))
                .WithMessage("Please use a valid UserName or Email");


            // Password
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please include a password");

            RuleFor(x => x.Password)
                .Length(Common.Constants.Users.minPasswordLength, Common.Constants.Users.maxPasswordLength)
                .When(x => !String.IsNullOrEmpty(x.Password))
                .WithMessage("Please use a valid password");

        }
    }
}