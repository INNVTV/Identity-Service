using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Roles.Models.Documents
{
    public class RoleDocumentModel
    {
        public RoleDocumentModel(string name, string description)
        {
            // Set our document partitioning property
            DocumentType = Common.Constants.DocumentType.PlatformRole();

            //Create our Id
            Id = Guid.NewGuid().ToString();

            Name = char.ToUpper(name[0]) + name.Substring(1); //<-- Make sure first character is capital
            NameKey = Common.Transformations.NameKey.Transform(name);
            Description = description;
        }

        [JsonProperty(PropertyName = "id")] //<-- Required for all Documents
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_docType")]
        public string DocumentType { get; internal set; } //<-- Our paritioning property

        public string Name { get; set; }
        public string NameKey { get; set; }
        public string Description { get; set; }

    }
}
