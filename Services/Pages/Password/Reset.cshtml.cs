using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [BindProperty]
        public ResetRequestModel ResetRequest { get; set; }

        readonly IMediator _mediator;

        public ResetModel(IServiceProvider serviceProvider)
        {
            _mediator = serviceProvider.GetService<IMediator>();
        }

        public async Task<IActionResult> OnGet(string resetCode)
        {
            ResetRequest = await _mediator.Send(new GetResetRequestQuery { ResetCode = resetCode });
            return Page();
        }
    }
}