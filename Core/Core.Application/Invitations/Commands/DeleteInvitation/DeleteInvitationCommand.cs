using Core.Common.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Commands.DeleteInvitation
{
    public class DeleteInvitationCommand : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }
}
