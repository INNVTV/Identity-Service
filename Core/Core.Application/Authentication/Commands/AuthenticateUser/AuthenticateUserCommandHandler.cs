using Core.Application.Authentication.Helpers;
using Core.Application.Users.Models.Documents;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using FluentValidation.Results;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Authentication.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserCommandResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public AuthenticateUserCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
        }

        public async Task<AuthenticateUserCommandResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            //Check local cache to see if too many login attempts have occured.
            #region Check login attempts (Redis Cache)


            #endregion 

            //_coreConfiguration.Logins.MaxAttemptsBeforeLockout;
            //_coreConfiguration.Logins.LockoutTimespanHours;
            //_coreConfiguration.Logins.AttemptsResetHours;

            AuthenticateUserValidator validator = new AuthenticateUserValidator(); 
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new AuthenticateUserCommandResponse(validationResult.Errors) { Message = "One or more validation errors occurred." };
            }



            //=========================================================================
            // DETERMINE AUTHENTICATION TYPE AND QUERY THE DOCUMENT STORE
            //=========================================================================

            #region get user from document store

            var authenticationType = "NameKey";
            var authenticationQuery = Common.Transformations.NameKey.Transform(request.UserNameOrEmail);

            if (request.UserNameOrEmail.Contains("@") && request.UserNameOrEmail.Contains("."))
            {
                authenticationType = "Email";
                authenticationQuery = request.UserNameOrEmail.ToLower().Trim();
            }

            // Create the query
            string sqlQuery = $"SELECT * FROM Documents d WHERE d.{ authenticationType } ='{ authenticationQuery }'";
            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.PlatformUser())
            };

            // Run query against the document store
            var getResult = _documentContext.Client.CreateDocumentQuery<UserDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions
            );

            var userDocumentModel = getResult.AsEnumerable().FirstOrDefault();

            if (userDocumentModel == null)
            {
                return new AuthenticateUserCommandResponse { Message = "Incorrect login." };
            }

            #endregion

            //=========================================================================
            // DETERMINE IF THE PASSWORD IS CORRECT
            //=========================================================================
            var authenticationGranted = Common.Hashing.PasswordHashing.ValidatePassword(request.Password, userDocumentModel.PasswordHash, userDocumentModel.PasswordSalt);

            if(!authenticationGranted)
            {
                return new AuthenticateUserCommandResponse { Message = "Incorrect login." };
                // TODO: Store Login Attempts for the UserNameOrEmail in cache. 
                #region Update login attempts for lockout (Redis Cache)

                #endregion

            }
            else
            {
                #region Clear login attempts (Redis Cache)


                #endregion

                //=========================================================================
                //
                //      BUILD THE JWT TOKEN AND RETURN RESULTS
                //
                //=========================================================================


                var rsaProvider = new RSACryptoServiceProvider();

                // Note: Requires the RsaCryptoExtensions.cs class in 'Helpers' folder (ToXMLString(true/flase) does not work in .Net Core so we have an extention method that parses pub/priv without boolean flag)
                rsaProvider.FromXmlRsaString(_coreConfiguration.JSONWebTokens.PrivateKeyXmlString);
                var rsaKey = new RsaSecurityKey(rsaProvider);

                var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

                List<Claim> claims = new List<Claim>()
                {
                    new Claim("id", userDocumentModel.Id),
                    new Claim("userName", userDocumentModel.UserName),
                    new Claim("nameKey", userDocumentModel.NameKey),
                    new Claim("email", userDocumentModel.Email),
                    new Claim("firstName", userDocumentModel.FirstName),
                    new Claim("lastName", userDocumentModel.LastName),
                };

                // Add roles to the claim
                foreach (var role in userDocumentModel.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var handler = new JwtSecurityTokenHandler();

                var jwtSecurityToken = handler.CreateJwtSecurityToken(
                    "IdentityService",
                    "IdentityConsumers",
                    new ClaimsIdentity(claims),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(_coreConfiguration.JSONWebTokens.ExpirationHours),
                    DateTime.UtcNow,
                    signingCredentials
                    );

                var jwtTokenString = handler.WriteToken(jwtSecurityToken);


                //=========================================================================
                // UPDATE LAST LOGIN DATETIME ON USER
                //=========================================================================

                #region Update Last Login DateTime

                // Update field:
                userDocumentModel.LastLoginDate = DateTime.UtcNow;

                var documentUri = UriFactory.CreateDocumentUri(
               _documentContext.Settings.Database,
               _documentContext.Settings.Collection,
               userDocumentModel.Id);

                ResourceResponse<Document> result;

                try
                {
                    // Save the document to document store using the IDocumentContext dependency
                    result = await _documentContext.Client.ReplaceDocumentAsync(
                        documentUri,
                        userDocumentModel,
                        new RequestOptions { PartitionKey = new PartitionKey(Common.Constants.DocumentType.PlatformUser().ToString()) }
                    );
                }
                catch (Exception ex)
                {
                    // throw DocumentStoreException (if a custom exception type is desired)
                    // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                    // Pass exception up the chain:
                    throw ex;

                    // ...Or pass along as inner exception:
                    //throw new Exception("An error occured trying to use the document store", ex);
                }
                finally
                {
                    // Close any open connections, etc...
                }

                #endregion

                //=========================================================================
                // LOG THE ACTIVITY
                //=========================================================================
                var user = AutoMapper.Mapper.Map<Core.Domain.Entities.User>(userDocumentModel);
                Log.Information("User authenticated {@user}", user);

                return new AuthenticateUserCommandResponse { isSuccess = true, JwtToken = jwtTokenString, User = user, Message = "Authentication succeeded" };
            }
        }

        
    }
}
