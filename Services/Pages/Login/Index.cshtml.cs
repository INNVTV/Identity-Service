using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Authentication.Commands.AuthenticateUser;
using Core.Infrastructure.Configuration;
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
        readonly ICoreConfiguration _coreConfiguration;

        public IndexModel(IServiceProvider serviceProvider, ICoreConfiguration coreConfiguration)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _coreConfiguration = coreConfiguration;
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
           
            // CROSS DOMAIN NOTE! ----------------
            // Note that you will need to be on the same domain to use this cookie on the application requiring authentication
            // To ease with local development it is recommended that you use the API endpoint to authenticate users from the application requring authentication.

            Response.Cookies.Append(
              "jwtToken",
              result.JwtToken,
              new CookieOptions()
              {
                  IsEssential = true,
                  HttpOnly = true,
                  Secure = true,
                  Expires = DateTime.UtcNow.AddHours(_coreConfiguration.JSONWebTokens.CookieExpirationHours),
                  SameSite = SameSiteMode.Strict
              });

            Response.Cookies.Append(
              "refreshToken",
              result.RefreshToken,
              new CookieOptions()
              {
                  IsEssential = true,
                  HttpOnly = true,
                  Secure = true,
                  Expires = DateTime.UtcNow.AddHours(_coreConfiguration.JSONWebTokens.CookieExpirationHours),
                  SameSite = SameSiteMode.Strict
              });
              
             // Redirect the user back to the client application
             return Redirect(Request.Query["returnUrl"]);

        }
    }
}