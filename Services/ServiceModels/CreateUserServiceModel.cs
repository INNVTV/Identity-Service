using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.ServiceModels
{
    [JsonObject(Title = "CreateUser")] //<-- Update name for OpenAPI/Swagger
    public class CreateUserServiceModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<string> Roles { get; set; }
    }
}
