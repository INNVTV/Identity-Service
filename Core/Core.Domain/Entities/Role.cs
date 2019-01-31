using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Role
    {
        public Role()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
        public string NameKey { get; set; }
        public string Description { get; set; }

    }
}
