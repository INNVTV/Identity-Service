using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Invitation
    {
        public Invitation()
        {
            Expired = false;
            DaysSinceCreated = 0;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }

        public int DaysSinceCreated { get; set; }
        public bool Expired { get; set; }
    }
}
