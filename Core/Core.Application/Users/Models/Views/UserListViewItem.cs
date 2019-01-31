using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Models.Views
{
    [JsonObject(Title = "User")] //<-- Update name for OpenAPI/Swagger
    public class UserListViewItem
    {
        public UserListViewItem()
        {
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NameKey { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
