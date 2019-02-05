using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Public
{
    public class KeysModel : PageModel
    {
        readonly ICoreConfiguration _coreConfiguration;

        public KeysModel(ICoreConfiguration coreConfiguration)
        {
            _coreConfiguration = coreConfiguration;
        }

        public void OnGet()
        {
            ViewData["XMLString"] = _coreConfiguration.JSONWebTokens.PublicKeyXmlString;
            ViewData["PEM"] = _coreConfiguration.JSONWebTokens.PublicKeyPEM;
        }
    }
}