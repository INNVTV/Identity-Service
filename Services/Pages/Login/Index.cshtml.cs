using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Authentication.Commands.AuthenticateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Pages.Login
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public AuthenticateUserCommand AuthenticateUser { get; set; }

        readonly IMediator _mediator;

        public IndexModel(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public void OnGet()
        {
            ViewData["Message"] = String.Empty;
            AuthenticateUser = new AuthenticateUserCommand();
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await _mediator.Send(AuthenticateUser);

            if(!result.isSuccess)
            {
                ViewData["Message"] = "Incorrect credentials";
                return Page();
            }

            return Redirect(Request.Query["redirect"]);

        }
    }
}