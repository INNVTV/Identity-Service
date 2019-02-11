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

namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IMediator _mediator;
        //private readonly ICoreConfiguration _coreConfiguration;

        public CreateUserValidator(IMediator mediator)//, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            //_coreConfiguration = coreConfiguration;

            // Email
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please include an email address");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !String.IsNullOrEmpty(x.Email))
                .WithMessage("Please enter a valid email address");

            RuleFor(x => x.Email)
                .Must(EmailNotExist)
                .When(x => !String.IsNullOrEmpty(x.Email))
                .WithMessage(x => $"Email '{x.Email}' already exists");

            // UserName
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Please include a UserName");

            RuleFor(x => x.UserName)
                .Length(Common.Constants.Users.minUserNameLength, Common.Constants.Users.maxUserNameLength)
                .When(x => !String.IsNullOrEmpty(x.UserName))
                .WithMessage($"UserName must be bewtween {Common.Constants.Users.minUserNameLength}-{Common.Constants.Users.maxUserNameLength} characters in length");

            RuleFor(x => x.UserName)
                .Must(BeAValidUserName)
                .When(x => !String.IsNullOrEmpty(x.UserName))
                .WithMessage(x => $"{x.UserName} is a reserved UserName");

            RuleFor(x => x.UserName)
                .Must(UserNameNotExist)
                .When(x => !String.IsNullOrEmpty(x.UserName))
                .WithMessage(x => $"UserName '{x.UserName}' already exists");


            // First Name
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please include a FirstName");

            RuleFor(x => x.FirstName)
                .Length(Common.Constants.Users.minFirstNameLength, Common.Constants.Users.maxFirstNameLength)
                .When(x => !String.IsNullOrEmpty(x.FirstName))
                .WithMessage($"FirstName must be bewtween {Common.Constants.Users.minFirstNameLength}-{Common.Constants.Users.maxFirstNameLength} characters in length");


            RuleFor(x => x.FirstName)
                .Must(Common.Validation.Methods.NotIncludeSpecialCharacters)
                .When(x => !String.IsNullOrEmpty(x.FirstName))
                .WithMessage(x => "FirstName cannot include special characters");

            // Last Name
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Please include a LastName");

            RuleFor(x => x.LastName)
                .Length(Common.Constants.Users.minLastNameLength, Common.Constants.Users.maxLastNameLength)
                .When(x => !String.IsNullOrEmpty(x.LastName))
                .WithMessage($"LastName must be bewtween {Common.Constants.Users.minLastNameLength}-{Common.Constants.Users.maxLastNameLength} characters in length");

            RuleFor(x => x.LastName)
                .Must(Common.Validation.Methods.NotIncludeSpecialCharacters)
                .When(x => !String.IsNullOrEmpty(x.LastName))
                .WithMessage(x => "LastName cannot include special characters");

            // Password
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please include a password");

            RuleFor(x => x.Password)
                .Length(Common.Constants.Users.minPasswordLength, Common.Constants.Users.maxPasswordLength)
                .When(x => !String.IsNullOrEmpty(x.Password))
                .WithMessage($"Password must be at least { Common.Constants.Users.minPasswordLength } characters in length");

            // Password Confirm
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Please confirm your password");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .When(x => !String.IsNullOrEmpty(x.ConfirmPassword))
                .WithMessage("Confirmation password does not match");

            // Roles
            RuleFor(x => x.Roles.Count)
                .GreaterThanOrEqualTo(1)
                .WithMessage("You must include at least one user role"); 
            
            RuleFor(x => x.Roles)
                .Must(MustIncludeValidRoles)
                .When(x => x.Roles.Count >= 1)
                .WithMessage(x => "Please include only valid roles");
        }

        private bool UserNameNotExist(string userName)
        {
            var userByUserNameQuery = new GetUserByUserNameQuery { UserName = Common.Transformations.NameKey.Transform(userName) };
            var userResults = _mediator.Send(userByUserNameQuery);

            if (userResults.Result != null)
            {
                return false;
            }
            
            return true;
        }

        private bool EmailNotExist(string email)
        {
            
            var userByEmailQuery = new GetUserByEmailQuery { Email = email.ToLower().Trim() };
            var userResults = _mediator.Send(userByEmailQuery);

            if (userResults.Result != null)
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

        public bool MustIncludeValidRoles(List<string> roleList)
        {
            var getRolesQuery = new GetRolesQuery();
            var result = _mediator.Send(getRolesQuery).Result;

            foreach(string role in roleList)
            {
                var validRole = result.Roles.Find(x => x.Name == role);
                if(validRole == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
