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


            // Password
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please include a password");

        }
    }
}