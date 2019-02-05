using Core.Common.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Commands.InviteUser
{
    public class InviteUserCommand : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
