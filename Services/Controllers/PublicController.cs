using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Core.Infrastructure.Configuration;
using IdentityService.ServiceModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Controllers
{
    [Route("api/public")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly ICoreConfiguration _coreConfiguration;

        public PublicController(IServiceProvider serviceProvider, ICoreConfiguration coreConfiguration)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _coreConfiguration = coreConfiguration;
        }

        [Route("keys")]
        [HttpGet]
        public PublicRsaKeysServiceModel Get()
        {
            var response = new PublicRsaKeysServiceModel
            {
                XMLString = _coreConfiguration.JSONWebTokens.PublicKeyXmlString,
                PEM = _coreConfiguration.JSONWebTokens.PublicKeyPEM
            };

            // Extract Modulus and Exponent from XMLString
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_coreConfiguration.JSONWebTokens.PublicKeyXmlString);

            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Modulus": response.Modulus = node.InnerText; break;
                    case "Exponent": response.Exponent = node.InnerText; break;
                }
            }

            return response;
        }
    }
}