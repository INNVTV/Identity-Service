using Core.Application.Invitations.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Queries.GetExpiredInvitations
{
    public class GetExpiredInvitationsQuery : IRequest<ExpiredInvitationsModel>
    {
        public GetExpiredInvitationsQuery()
        {
            //Default Query Options
            PageSize = 20;
            ContinuationToken = null;
        }

        public int PageSize { get; set; }
        public string ContinuationToken; //<-- Null on first call
    }
}
