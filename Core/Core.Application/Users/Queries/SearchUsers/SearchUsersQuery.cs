using Core.Application.Users.Models.Enums;
using Core.Application.Users.Models.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Queries.SearchUsers
{
    public class SearchUsersQuery : IRequest<UserSearchResultsViewModel>
    {
        public SearchUsersQuery()
        {
            //Default Search Options
            QueryString = "";
            Page = 1;
            PageSize = 20;
            OrderBy = OrderBy.UserName;
            OrderDirection = OrderDirection.ASC;
        }

        public string QueryString { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public OrderBy OrderBy { get; set; }
        public OrderDirection OrderDirection { get; set; }

    }
}
