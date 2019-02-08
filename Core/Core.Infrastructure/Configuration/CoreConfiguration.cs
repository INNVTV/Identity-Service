using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure.Configuration
{
    /// <summary>
    /// We use the ICoreConfiguration type as a way to inject settings
    /// and resource connections into our classes from our main entry points.
    /// </summary>
    public class CoreConfiguration : ICoreConfiguration
    {
        public CoreConfiguration(IConfiguration configuration)
        {
            // New up our root classes
            Application = new ApplicationSettings();
            Security = new SecuritySettings();
            Hosting = new HostingConfiguration();
            JSONWebTokens = new JWTConfiguration();
            Endpoints = new EndpointSettings();
            Invitations = new InvitationSettings();
            Logins = new LoginSettings(); 

            // Map appsettings.json
            Application.Name = configuration.GetSection("Application").GetSection("Name").Value;
            Security.PrimaryApiKey = configuration.GetSection("Security").GetSection("PrimaryApiKey").Value;
            Security.SecondaryApiKey = configuration.GetSection("Security").GetSection("SecondaryApiKey").Value;

            JSONWebTokens.ExpirationHours = Convert.ToInt32(configuration.GetSection("JWT").GetSection("ExpirationHours").Value);
            JSONWebTokens.PrivateKeyXmlString = configuration.GetSection("JWT").GetSection("PrivateKeyXmlString").Value;
            JSONWebTokens.PublicKeyXmlString = configuration.GetSection("JWT").GetSection("PublicKeyXmlString").Value;
            JSONWebTokens.PublicKeyPEM = configuration.GetSection("JWT").GetSection("PublicKeyPEM").Value;

            Endpoints.Domain = configuration.GetSection("Endpoints").GetSection("Domain").Value;

            Invitations.ExpirationDays = Convert.ToInt32(configuration.GetSection("Invitations").GetSection("ExpirationDays").Value);


            Logins.MaxAttemptsBeforeLockout = Convert.ToInt32(configuration.GetSection("Login").GetSection("MaxAttemptsBeforeLockout").Value);
            Logins.LockoutTimespanHours = Convert.ToInt32(configuration.GetSection("Login").GetSection("LockoutTimespanHours").Value);
            Logins.PasswordResetTimespanHours = Convert.ToInt32(configuration.GetSection("Login").GetSection("PasswordResetTimespanHours").Value);

            #region Hosting configuration details (if available)

            try
            {
                // Azure WebApp provides these settings when deployed.
                Hosting.SiteName = configuration["WEBSITE_SITE_NAME"];
                Hosting.InstanceId = configuration["WEBSITE_INSTANCE_ID"];
            }
            catch
            {
            }


            #endregion


        }

        public ApplicationSettings Application { get; set; }
        public SecuritySettings Security { get; set; }
        public HostingConfiguration Hosting { get; set; }
        public JWTConfiguration JSONWebTokens { get; set; }
        public EndpointSettings Endpoints { get; set; }
        public InvitationSettings Invitations { get; set; }
        public LoginSettings Logins { get; set; }
    }
}
