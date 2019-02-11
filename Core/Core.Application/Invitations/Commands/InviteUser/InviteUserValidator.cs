using Core.Application.Roles.Queries.GetRoles;
using Core.Application.Users.Queries.GetUserByEmail;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Commands.InviteUser
{
    public class InviteUserValidator : AbstractValidator<InviteUserCommand>
    {
        private readonly IMediator _mediator;
        //private readonly ICoreConfiguration _coreConfiguration;

        public InviteUserValidator(IMediator mediator)//, ICoreConfiguration coreConfiguration)
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

            // Roles
            RuleFor(x => x.Roles.Count)
                .GreaterThanOrEqualTo(1)
                .WithMessage("You must include at least one user role");

            RuleFor(x => x.Roles)
                .Must(MustIncludeValidRoles)
                .When(x => x.Roles.Count >= 1)
                .WithMessage(x => "Please include only valid roles");
        }

        private bool EmailNotExist(string email)
        {

            var userByEmailQuery = new GetUserByEmailQuery { Email = email.ToLower().Trim() };
            var userResult = _mediator.Send(userByEmailQuery);

            if (userResult.Result != null)
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

            foreach (string role in roleList)
            {
                var validRole = result.Roles.Find(x => x.Name == role);
                if (validRole == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
