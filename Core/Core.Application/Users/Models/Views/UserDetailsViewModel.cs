using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Models.Views
{
    public class UserDetailsViewModel
    {
        public UserDetailsViewModel()
        {
            EditEnabled = false;
            DeleteEnabled = false;
        }

        public User User { get; set; }

        public bool EditEnabled { get; set; }
        public bool DeleteEnabled { get; set; }
    }
}
