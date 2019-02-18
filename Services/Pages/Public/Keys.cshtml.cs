using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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

            // Extract Modulus and Exponent from XMLString
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_coreConfiguration.JSONWebTokens.PublicKeyXmlString);

            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Modulus": ViewData["Modulus"] = node.InnerText; break;
                    case "Exponent": ViewData["Exponent"] = node.InnerText; break;
                }
            }
        }
    }
}