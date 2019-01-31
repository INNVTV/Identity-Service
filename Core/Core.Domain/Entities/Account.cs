using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Account
    {
        public Account()
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string NameKey { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
