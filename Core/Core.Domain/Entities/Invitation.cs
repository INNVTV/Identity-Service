using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Invitation
    {
        public Invitation()
        {
            IsExpired = false;
            DaysSinceCreated = 0;
            Roles = new List<string>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime CreatedDate { get; set; }

        public int DaysSinceCreated { get; set; }
        public bool IsExpired { get; set; }
    }
}
