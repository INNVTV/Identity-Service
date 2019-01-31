using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Core.Common.Constants
{
    /// <summary>
    /// These are the default roles in the system and cannot be deleted. They are created on startup.
    /// </summary>
    public static class Roles
    {
        public static List<Role> DefaultRoles = new List<Role> {
            new Role { Name = "Owner", Description = "The owner" },
            new Role { Name = "Admin", Description = "Administrator role" },
            new Role { Name = "Manager", Description = "Manager role" },
            new Role { Name = "User", Description = "User role" },
            new Role { Name = "Reader", Description = "Read only role" }
        }; 
    }
}
