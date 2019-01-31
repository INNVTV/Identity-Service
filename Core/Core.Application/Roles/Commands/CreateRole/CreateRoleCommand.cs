using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<CreateRoleCommandResponse>
    {
        public string Name;
        public string Description;
    }
}
