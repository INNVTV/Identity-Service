using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.ServiceModels
{
    public class PublicRsaKeysServiceModel
    {
        public string XMLString { get; set; }
        public string PEM { get; set; }
        public string Modulus { get; set; }
        public string Exponent { get; set; }
    }
}
