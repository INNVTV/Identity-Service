using Core.Application.Roles.Queries.GetRole;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Application.Roles.Commands.CreateRole
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        private readonly IMediator _mediator;

        public CreateRoleValidator(IMediator mediator)
        {
            _mediator = mediator;

            // Role Name
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please include a name for your new role");

            RuleFor(x => x.Name)
                .Length(2, 40)
                .When(x => !String.IsNullOrEmpty(x.Name))
                .WithMessage("Role name must be bewtween 2-24 characters in length");

            RuleFor(x => x.Name)
                .Must(NotIncludeNumbersOrSpecialCharacters)
                .When(x => !String.IsNullOrEmpty(x.Name))
                .WithMessage("Role cannot contain numbers, spaces or special characters");

            RuleFor(x => x.Name)
                .Must(BeAValidName)
                .When(x => !String.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} is a reserved role name");

            RuleFor(x => x.Name)
                .Must(NotExist)
                .When(x => !String.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"Role named '{x.Name}' already exists");
        }

        private object RuleFor(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        private bool NotExist(string name)
        {
            //=========================================================================
            // VALIDATE ROLE NAME IS UNIQUE (Via MediatR Query)
            //=========================================================================
            // Note: "NameKey" is transformed from "Name" and is used as a both a unique id as well as for pretty routes/urls
            // Note: Consider using both "Name and ""NameKey" as UniqueKeys on your DocumentDB collection.
            //-------------------------------------------------------------------------
            // Note: Once these contraints are in place you could remove this manual check
            //  - however this process does ensure no exceptions are thrown and a cleaner response message is sent to the user.
            //----------------------------------------------------------------------------

            var getRoleQuery = new GetRoleQuery { NameKey = Common.Transformations.NameKey.Transform(name) };
            var role = _mediator.Send(getRoleQuery);

            if (role.Result.Role != null)
            {
                return false;
            }

            return true;
        }

        private bool NotIncludeNumbersOrSpecialCharacters(string name)
        {
            var regex = new Regex("^[a-zA-Z]*$");

            if (regex.IsMatch(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool BeAValidName(string name)
        {
            // custom code validating logic goes here
            foreach (string reservedName in ReservedRoleNames)
            {
                if (reservedName == Common.Transformations.NameKey.Transform(name))
                {
                    return false;
                }
            }

            return true;
        }

        private static readonly ReadOnlyCollection<string> ReservedRoleNames = new ReadOnlyCollection<string>(new[]
        {
            #region Reserved Role Names
            
            //a
            "xxx",

            #endregion

        });
    }
}
