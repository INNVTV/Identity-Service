using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Queries.GetInvitationById
{
    public class GetInvitationByIdQuery : IRequest<Invitation>
    {
        public string Id { get; set; }
    }
}
