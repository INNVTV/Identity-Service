using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Passwords.Commands.ForgotPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Pages.Password
{
    public class ForgotModel : PageModel
    {
        [BindProperty]
        public string UserNameOrEmail { get; set; }

        readonly IMediator _mediator;

        public ForgotModel(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public void OnGet()
        {
            ViewData["Message"] = String.Empty;
        }

        public async Task<IActionResult> OnPost()
        {
            if(!String.IsNullOrEmpty(UserNameOrEmail))
            {
                await _mediator.Send(new ForgotPasswordCommand { UserNameOrEmail = UserNameOrEmail });
                return RedirectToPage("Sent");
            }
            else
            {
                ViewData["Message"] = "Please submit a username or password!";
                return Page();
            }
        }
    }
}