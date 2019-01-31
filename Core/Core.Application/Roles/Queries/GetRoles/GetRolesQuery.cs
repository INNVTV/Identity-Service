using Core.Application.Roles.Models.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Roles.Queries.GetRoles
{
    public class GetRolesQuery : IRequest<RolesViewModel>
    {
    }
}
