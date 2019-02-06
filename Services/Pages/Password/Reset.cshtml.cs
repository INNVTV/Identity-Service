using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Password
{
    public class ResetModel : PageModel
    {
        public void OnGet(string id)
        {
            var _id = id;
        }
    }
}