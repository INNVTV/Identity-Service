using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Invitations.Queries.GetInvitationById;
using Core.Application.Users.Commands.CreateUser;
using Core.Infrastructure.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Pages
{
    public class InvitationModel : PageModel
    {

        [BindProperty]
        public string Id { get; set; }

        [BindProperty]
        public Core.Domain.Entities.Invitation Invitation { get; set; }

        [BindProperty]
        public CreateUserCommand CreateUserCommandForm { get; set; }

        [BindProperty]
        public CreateUserCommandResponse CreateUserFormResponse { get; set; }

        readonly IMediator _mediator;

        public InvitationModel(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public async Task OnGet(string id)
        {
            Id = id;

            try
            {
                Invitation = await _mediator.Send(new GetInvitationByIdQuery { Id = id });
            }
            catch
            {

            }
        }

        public async Task OnPost()
        {
            CreateUserFormResponse = await _mediator.Send(CreateUserCommandForm);         
        }
    }
}