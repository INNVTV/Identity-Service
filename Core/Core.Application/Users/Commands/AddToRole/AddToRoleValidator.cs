using Core.Application.Roles.Queries.GetRole;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.AddToRole
{
    public class AddToRoleValidator : AbstractValidator<AddToRoleCommand>
    {
        private readonly IMediator _mediator;

        public AddToRoleValidator(IMediator mediator)
        {
            _mediator = mediator;

            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an id");
            RuleFor(x => x.RoleToAdd).NotEmpty().WithMessage("Please specify a role to add");

            RuleFor(x => x.RoleToAdd)
                .Must(Exist)
                .When(x => !String.IsNullOrEmpty(x.RoleToAdd))
                .WithMessage(x => $"{x.RoleToAdd} is not a valid role");
        }

        private bool Exist(string roleToAdd)
        {

            var getRoleQuery = new GetRoleQuery { NameKey = Common.Transformations.NameKey.Transform(roleToAdd) };
            var role = _mediator.Send(getRoleQuery);

            if (role.Result.Role == null)
            {
                return false;
            }
            else
            {
                // We also need to make sure the casing is accurate
                if (role.Result.Role.Name != roleToAdd)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
