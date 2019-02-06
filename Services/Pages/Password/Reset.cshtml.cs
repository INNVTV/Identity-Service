using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Passwords.Commands.ResetPassword;
using Core.Application.Passwords.Models;
using Core.Application.Passwords.Queries.GetResetRequest;
using IdentityService.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Pages.Password
{
    public class ResetModel : PageModel
    {
        [BindProperty]
        public ResetPasswordViewModel ResetPassword { get; set; }


        readonly IMediator _mediator;

        public ResetModel(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public async Task<IActionResult> OnGet(string resetCode)
        {
            var result = await _mediator.Send(new GetResetRequestQuery { ResetCode = resetCode });

            ResetPassword = new ResetPasswordViewModel();

            ResetPassword.ResetRequestValid = result.IsValid;
            if(result.IsValid)
            {
                ResetPassword.ResetCode = resetCode;
                ResetPassword.UserId = result.UserId;
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var response = await _mediator.Send(new ResetPasswordCommand { ResetCode = ResetPassword.ResetCode, UserId = ResetPassword.UserId, NewPassword = ResetPassword.NewPassword, ConfirmNewPassword = ResetPassword.ConfirmNewPassword });

            if(response.isSuccess)
            {
                return RedirectToPage("Success");
            }
            else
            {
                if (response.ValidationIssues != null)
                {
                    foreach (var validationProperty in response.ValidationIssues)
                    {
                        foreach (var propertyFailure in validationProperty.PropertyFailures)
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