using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.DeleteUser
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an id");
        }
    }
}
