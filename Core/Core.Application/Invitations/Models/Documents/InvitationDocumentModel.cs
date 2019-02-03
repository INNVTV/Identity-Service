using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Invitations.Models.Documents
{
    public class InvitationDocumentModel
    {
        public InvitationDocumentModel(string email, List<string> roles)
        {
            // Set our document partitioning property
            DocumentType = Common.Constants.DocumentType.Invitation();

            //Create our Id
            Id = Guid.NewGuid().ToString();

            CreatedDate = DateTime.UtcNow;

            if (!String.IsNullOrEmpty(email))
            {
                Email = email.ToLower().Trim();
            }

            Roles = roles;
        }

        [JsonProperty(PropertyName = "id")] //<-- Required for all Documents
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_docType")]
        public string DocumentType { get; internal set; } //<-- Our paritioning property

        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Roles { get; set; }

    }
}
