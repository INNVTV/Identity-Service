using Core.Application.Roles.Models.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Roles.Queries.GetRole
{
    public class GetRoleQuery : IRequest<RoleViewModel>
    {
        public string NameKey { get; set; }
    }
}
