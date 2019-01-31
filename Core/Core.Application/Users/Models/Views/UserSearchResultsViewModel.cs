using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Models.Views
{
    public class UserSearchResultsViewModel
    {
        public UserSearchResultsViewModel()
        {
            Users = new List<UserListViewItem>();
            EditEnabled = false;
            DeleteEnabled = false;

        }

        public List<UserListViewItem> Users { get; set; }

        public int TotalResults;
        public int NextAmount;
        public int Page;
        public int PageSize;

        public bool EditEnabled { get; set; }
        public bool DeleteEnabled { get; set; }
    }
}
