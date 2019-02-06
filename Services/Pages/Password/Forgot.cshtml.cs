using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Password
{
    public class ForgotModel : PageModel
    {
        [BindProperty]
        public string UserNameOrEmail { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            return Page();
        }
    }
}