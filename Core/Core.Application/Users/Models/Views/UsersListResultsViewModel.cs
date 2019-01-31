using Core.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Models.Views
{
    [JsonObject(Title="UserList")] //<-- Update name for OpenAPI/Swagger 
    public class UsersListResultsViewModel
    {
        public UsersListResultsViewModel()
        {
            Users = new List<UserListViewItem>();
            HasMoreResults = false;
            ContinuationToken = "";
            EditEnabled = false;
            DeleteEnabled = false;
            Count = 0;
        }

        public List<UserListViewItem> Users { get; set; }

        public bool EditEnabled { get; set; }
        public bool DeleteEnabled { get; set; }

        public int Count;
        public bool HasMoreResults;
        public string ContinuationToken { get; set; } //<-- Use for next call. Null on final
        public object User { get; internal set; }
    }
}
