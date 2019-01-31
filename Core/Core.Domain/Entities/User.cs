using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class User
    {
        public User()
        {
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public List<string> Roles { get; set; }
    }
}
