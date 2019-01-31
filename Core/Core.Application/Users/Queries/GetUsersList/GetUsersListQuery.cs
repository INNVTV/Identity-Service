using Core.Application.Users.Models.Enums;
using Core.Application.Users.Models.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Queries.GetUsersList
{
    public class GetUsersListQuery : IRequest<UsersListResultsViewModel>
    {
        public GetUsersListQuery()
        {
            //Default Query Options
            PageSize = 20;
            OrderBy = OrderBy.UserName;
            OrderDirection = OrderDirection.ASC;
            ContinuationToken = null;
        }

        public int PageSize { get; set; }
        public OrderBy OrderBy { get; set; }
        public OrderDirection OrderDirection { get; set; }

        public string ContinuationToken; //<-- Null on first call
    }
}
