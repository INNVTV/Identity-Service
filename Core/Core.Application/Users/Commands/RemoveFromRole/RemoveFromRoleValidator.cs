using Core.Application.Roles.Queries.GetRole;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.RemoveFromRole
{
    public class RemoveFromRoleValidator : AbstractValidator<RemoveFromRoleCommand>
    {
        private readonly IMediator _mediator;

        public RemoveFromRoleValidator(IMediator mediator)
        {
            _mediator = mediator;

            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an id");
            RuleFor(x => x.RoleToRemove).NotEmpty().WithMessage("Please specify a role to remove");

            RuleFor(x => x.RoleToRemove)
                .Must(Exist)
                .When(x => !String.IsNullOrEmpty(x.RoleToRemove))
                .WithMessage(x => $"{x.RoleToRemove} is not a valid role");
        }

        private bool Exist(string roleToRemove)
        {

            var getRoleQuery = new GetRoleQuery { NameKey = Common.Transformations.NameKey.Transform(roleToRemove) };
            var role = _mediator.Send(getRoleQuery);

            if (role.Result.Role == null)
            {
                return false;
            }
            else
            {
                // We also need to make sure the casing is accurate
                if (role.Result.Role.Name != roleToRemove)
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
