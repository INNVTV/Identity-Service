using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Models
{
    public class ExpiredInvitationsModel
    {
        public ExpiredInvitationsModel()
        {
            Ids = new List<string>();
            HasMoreResults = false;
            ContinuationToken = "";
            Count = 0;
        }

        public List<string> Ids { get; set; }


        public int Count;
        public bool HasMoreResults;
        public string ContinuationToken { get; set; } //<-- Use for next call. Null on final
    }
}
