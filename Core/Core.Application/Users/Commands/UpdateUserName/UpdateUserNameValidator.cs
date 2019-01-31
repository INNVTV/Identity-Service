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

namespace Core.Application.Users.Commands.UpdateUserName
{
    public class UpdateUserNameValidator : AbstractValidator<UpdateUserNameCommand>
    {
        private readonly IMediator _mediator;
        //private readonly ICoreConfiguration _coreConfiguration;

        public UpdateUserNameValidator(IMediator mediator)//, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            //_coreConfiguration = coreConfiguration;


            // NewUserName
            RuleFor(x => x.NewUserName).NotEmpty().WithMessage("Please include a UserName");

            RuleFor(x => x.NewUserName)
                .Length(Common.Constants.Users.minUserNameLength, Common.Constants.Users.maxUserNameLength)
                .When(x => !String.IsNullOrEmpty(x.NewUserName))
                .WithMessage($"UserName must be bewtween {Common.Constants.Users.minUserNameLength}-{Common.Constants.Users.maxUserNameLength} characters in length");

            RuleFor(x => x.NewUserName)
                .Must(BeAValidUserName)
                .When(x => !String.IsNullOrEmpty(x.NewUserName))
                .WithMessage(x => $"{x.NewUserName} is a reserved UserName");

            RuleFor(x => x.NewUserName)
                .Must(UserNameNotExist)
                .When(x => !String.IsNullOrEmpty(x.NewUserName))
                .WithMessage(x => $"UserName '{x.NewUserName}' already exists");
        }

        private bool UserNameNotExist(string userName)
        {
            var userByUserNameQuery = new GetUserByUserNameQuery { UserName = Common.Transformations.NameKey.Transform(userName) };
            var userDetails = _mediator.Send(userByUserNameQuery);

            if (userDetails.Result.User != null)
            {
                return false;
            }
            
            return true;
        }


        private bool BeAValidUserName(string name)
        {
            foreach (string reservedName in Common.Validation.ReservedNames.ReservedUserNames)
            {
                if (reservedName == Common.Transformations.NameKey.Transform(name))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
