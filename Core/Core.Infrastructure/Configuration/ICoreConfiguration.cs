using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using StackExchange.Redis;
using System.Security;

namespace Core.Infrastructure.Configuration
{
    public interface ICoreConfiguration
    {
        ApplicationSettings Application { get; set; }
        SecuritySettings Security { get; set; }
        JWTConfiguration JSONWebTokens { get; set; }


        EndpointSettings Endpoints { get; set; }
        InvitationSettings Invitations { get; set; }

        LoginSettings Logins { get; set; }

        HostingConfiguration Hosting { get; set; }

    }

    #region Classes

    #region Application

    public class ApplicationSettings
    {
        public string Name { get; set; }
    }


    #endregion

    #region Security

    public class SecuritySettings
    {
        public string PrimaryApiKey { get; set; }
        public string SecondaryApiKey { get; set; }

        public bool ForceSecureApiCalls { get; set; }
    }


    #endregion

    #region Cookies




    #endregion

    #region JSONWebTokens

    public class JWTConfiguration
    {
        public int TokenExpirationHours { get; set; }
        public int RefreshTokenExpirationHours { get; set; }

        public string Issuer { get; set; }
        public string Audience { get; set; }

        public string RefreshTokenEncryptionPassPhrase { get; set; }
        public string PrivateKeyXmlString { get; set; }
        public string PublicKeyXmlString { get; set; }
        public string PublicKeyPEM { get; set; }
    }


    #endregion

    #region Endpoints

    public class EndpointSettings
    {
        public string ClientDomain { get; set; }
        public string PasswordResetPath { get; set; }
        public string AcceptInvitationsPath { get; set; }
    }

    #endregion

    #region Invitations

    public class InvitationSettings
    {
        public int ExpirationDays { get; set; }
    }

    #endregion

    #region Logins

    public class LoginSettings
    {
        public int MaxAttemptsBeforeLockout { get; set; }
        public int LockoutTimespanHours { get; set; }
        public int PasswordResetTimespanHours { get; set; }
    }

    #endregion

    #region Hosting

    /// <summary>
    /// Only used in Azure WebApp hosted deployments.
    /// Returns info on the WebApp instance for the current process. 
    /// Can be used to log which WebApp instance a process ran on.
    /// </summary>
    public class HostingConfiguration
    {

        public string SiteName { get; set; }
        public string InstanceId { get; set; }
    }

    #endregion

    #endregion
}
