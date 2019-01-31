using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Models.Documents
{
    public class UserDocumentModel
    {
        public UserDocumentModel(string firstName, string lastName, string userName, string email, List<string> roles, string passwordSalt, string passwordHash)
        {
            // Set our document partitioning property
            DocumentType = Common.Constants.DocumentType.User();

            //Create our Id
            Id = Guid.NewGuid().ToString();
            
            FirstName = firstName;
            LastName = lastName;

            UserName = userName;
            NameKey = Common.Transformations.NameKey.Transform(userName);
            CreatedDate = DateTime.UtcNow;
            LastLoginDate = DateTime.UtcNow;

            if(!String.IsNullOrEmpty(email))
            {
                Email = email.ToLower().Trim();
            }      

            Roles = roles;
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;

            Active = true;
        }

        [JsonProperty(PropertyName = "id")] //<-- Required for all Documents
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_docType")]
        public string DocumentType { get; internal set; } //<-- Our paritioning property

        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NameKey { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate {get; set;}
        public DateTime LastLoginDate { get; set; }

        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }

        public List<string> Roles { get; set; }

    }

}
