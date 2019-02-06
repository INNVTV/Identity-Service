using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.ViewModels
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel()
        {
            ResetRequestValid = true;
        }

        public bool ResetRequestValid { get; set; }

        public string ResetCode { get; set; }
        public string UserId { get; set; }


        [Display(Name = "New Password:")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password:")]
        public string ConfirmNewPassword { get; set; }
    }
}
