using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Invitations.Commands.DeleteInvitation;
using Core.Application.Invitations.Queries.GetInvitationById;
using Core.Application.Users.Commands.CreateUser;
using Core.Infrastructure.Configuration;
using IdentityService.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Pages.Invitations
{
    public class AcceptModel : PageModel
    {

        [BindProperty]
        public AcceptInvitationViewModel AcceptInvitation { get; set; }

        readonly IMediator _mediator;

        public AcceptModel(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public async Task OnGet(string id)
        {
            try
            {
                var invitation = await _mediator.Send(new GetInvitationByIdQuery { Id = id });
                if(invitation != null && invitation.Id == id)
                {
                    AcceptInvitation = new AcceptInvitationViewModel
                    {             
                        InvitationExists = true,
                        InvitationId = invitation.Id,
                        InvitationExpired = invitation.IsExpired
                    };
                }
                else
                {
                    AcceptInvitation = new AcceptInvitationViewModel
                    {
                        InvitationExists = false
                    };
                }
            }
            catch
            {
                AcceptInvitation = new AcceptInvitationViewModel {
                    InvitationExists = false
                };
            }
        }

        public async Task<IActionResult> OnPost()
        {

            var invitation = await _mediator.Send(new GetInvitationByIdQuery { Id = AcceptInvitation.InvitationId });

            var response = await _mediator.Send(new CreateUserCommand
            {
                // From Document Store
                Email = invitation.Email,
                Roles = invitation.Roles,

                // From User Submission
                UserName = AcceptInvitation.UserName,
                FirstName = AcceptInvitation.FirstName,
                LastName = AcceptInvitation.LastName,
                Password = AcceptInvitation.Password,
                ConfirmPassword = AcceptInvitation.ConfirmPassword
            });

            if (response.isSuccess)
            {
                // Delete invitation
                await _mediator.Send(new DeleteInvitationCommand { Id = AcceptInvitation.InvitationId });

                // Send to success page
                return RedirectToPage("Success");
            }
            else
            {
                if(response.ValidationIssues != null)
                {
                    foreach(var validationProperty in response.ValidationIssues)
                    {
                        foreach(var propertyFailure in validationProperty.PropertyFailures)
                        {
                            ModelState.AddModelError(validationProperty.PropertyName, propertyFailure);
                        }
                    }

                    return Page();
                }

                return Page();
            }

        }
    }
}