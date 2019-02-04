using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.ViewModels
{
    public class AcceptInvitationViewModel
    {
        public AcceptInvitationViewModel()
        {
            InvitationExists = false;
            InvitationExpired = true;

        }

        public bool InvitationExists { get; set; }
        public bool InvitationExpired { get; set; }
        
        public string InvitationId { get; set; }

        [Display(Name = "Username:")]
        public string UserName { get; set; }

        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        [Display(Name = "Password:")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password:")]
        public string ConfirmPassword { get; set; }
    }
}
