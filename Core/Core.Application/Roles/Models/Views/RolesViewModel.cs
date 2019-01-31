using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Roles.Models.Views
{
    public class RolesViewModel
    {
        public RolesViewModel()
        {
            Roles = new List<Role>();
        }

        public List<Role> Roles { get; set; }

    }
}
