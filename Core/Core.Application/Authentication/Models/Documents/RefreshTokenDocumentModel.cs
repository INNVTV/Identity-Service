using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Models.Documents
{
    public class RefreshTokenDocumentModel
    {
        public RefreshTokenDocumentModel(string userId)
        {
            // Set our document partitioning property
            DocumentType = Common.Constants.DocumentType.RefreshToken();

            //Create our Id
            Id = Guid.NewGuid().ToString();
            NameKey = Id; //<--our default for documents that won't need a pretty route

            UserId = Guid.NewGuid().ToString();
            CreatedDate = DateTime.UtcNow;

        }

        [JsonProperty(PropertyName = "id")] //<-- Required for all Documents
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_docType")]
        public string DocumentType { get; internal set; } //<-- Our paritioning property

        public string NameKey { get; internal set; } //<-- NameKey is our additional index constrain on DocumentDB. Use a value that MUST be unique on all documents within a given partition

        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
