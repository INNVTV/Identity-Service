using FluentValidation;

namespace Core.Application.Invitations.Commands.DeleteInvitation
{
    public class DeleteInvitationValidator : AbstractValidator<DeleteInvitationCommand>
    {
        public DeleteInvitationValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an id");
        }
    }
}