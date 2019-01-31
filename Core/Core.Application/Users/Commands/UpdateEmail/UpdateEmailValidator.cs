using Core.Application.Roles.Queries.GetRoles;
using Core.Application.Users.Queries.GetUserByEmail;
using Core.Application.Users.Queries.GetUserByUserName;
using Core.Infrastructure.Configuration;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Application.Users.Commands.UpdateEmail
{
    public class UpdateEmailValidator : AbstractValidator<UpdateEmailCommand>
    {
        private readonly IMediator _mediator;
        //private readonly ICoreConfiguration _coreConfiguration;

        public UpdateEmailValidator(IMediator mediator)//, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            //_coreConfiguration = coreConfiguration;


            // NewUserName
            RuleFor(x => x.NewEmail).NotEmpty().WithMessage("Please include a UserName");

            RuleFor(x => x.NewEmail)
                .Length(Common.Constants.Users.minUserNameLength, Common.Constants.Users.maxUserNameLength)
                .When(x => !String.IsNullOrEmpty(x.NewEmail))
                .WithMessage($"UserName must be bewtween {Common.Constants.Users.minUserNameLength}-{Common.Constants.Users.maxUserNameLength} characters in length");

            RuleFor(x => x.NewEmail)
                .EmailAddress()
                .When(x => !String.IsNullOrEmpty(x.NewEmail))
                .WithMessage("Please enter a valid email address");

            RuleFor(x => x.NewEmail)
                .Must(EmailNotExist)
                .When(x => !String.IsNullOrEmpty(x.NewEmail))
                .WithMessage(x => $"Email '{x.NewEmail}' already exists");
        }

        private bool EmailNotExist(string email)
        {
            var userByEmailQuery = new GetUserByEmailQuery { Email = email.ToLower().Trim() };
            var userDetails = _mediator.Send(userByEmailQuery);

            if (userDetails.Result.User != null)
            {
                return false;
            }
            
            return true;
        }

    }
}
