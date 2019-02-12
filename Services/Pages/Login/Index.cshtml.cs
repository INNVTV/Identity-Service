using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Authentication.Commands.AuthenticateUser;
using MediatR;
using Microsoft.AspNetCore.Http;
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
                ViewData["Message"] = result.Message;
                return Page();
            }

            // Store our JWT token and RefreshToken as cookies
        
            Response.Cookies.Append(
              "jwtToken",
              result.JwtToken,
              new CookieOptions()
              {
                  IsEssential = true,
                  HttpOnly = true,
                  Secure = true
              });

            Response.Cookies.Append(
              "refreshToken",
              result.RefreshToken,
              new CookieOptions()
              {
                  IsEssential = true,
                  HttpOnly = true,
                  Secure = true
              });


            // Redirect the user back to the client application
            return Redirect(Request.Query["returnUrl"]);

        }
    }
}