using Core.Application.Authentication.Commands.GenerateRefreshToken;
using Core.Application.Authentication.Helpers;
using Core.Application.Authentication.Models;
using Core.Application.Authentication.Models.Documents;
using Core.Application.Users.Queries.GetUserById;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using FluentValidation.Results;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Authentication.Commands.AuthenticateRefreshToken
{
    public class AuthenticateRefreshTokenCommandHandler : IRequestHandler<AuthenticateRefreshTokenCommand, AuthenticationResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public AuthenticateRefreshTokenCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
        }

        public async Task<AuthenticationResponse> Handle(AuthenticateRefreshTokenCommand request, CancellationToken cancellationToken)
        {

            // Validate our input
            AuthenticateRefreshTokenValidator validator = new AuthenticateRefreshTokenValidator();
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new AuthenticationResponse(validationResult.Errors) { Message = "Validation issues" };
            }



            //======================================================
            // QUERY THE DOCUMENT STORE FOR BOTH THE REFRESH TOKEN 
            //======================================================

            #region get request token from document store

            // Create the query
            string sqlQuery = $"SELECT * FROM Documents d WHERE d.id ='{ request.TokenString }'";
            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.RefreshToken())
            };

            // Run query against the document store
            var getResult = _documentContext.Client.CreateDocumentQuery<RefreshTokenDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions
            );

            var refreshDocumentModel = getResult.AsEnumerable().FirstOrDefault();

            #endregion

            //=========================================================================
            // DETERMINE IF THE TOKEN EXISTS AND IS NOT EXPIRED
            //=========================================================================

            if (refreshDocumentModel == null)
            {
                return new AuthenticationResponse { Message = "Invalid Token" };
            }
            else if (refreshDocumentModel.CreatedDate.AddHours(_coreConfiguration.JSONWebTokens.RefreshExpirationHours) <= DateTime.UtcNow)
            {
                return new AuthenticationResponse { Message = "Invalid Token" };
            }
            else
            {
                //=========================================================================
                // TOKEN IS VALID, BUILD OUR AUTHENTICATION RESPONSE
                //=========================================================================

                // Get the user
                var userViewModel = await _mediator.Send(new GetUserByIdQuery { Id = refreshDocumentModel.UserId });

                //=========================================================================
                //
                //      BUILD THE JWT TOKEN, REFRESH TOKEN AND RETURN RESULTS
                //
                //=========================================================================

                if(userViewModel == null || userViewModel.User == null)
                {
                    return new AuthenticationResponse { Message = "Invalid Token" };
                }

                var user = userViewModel.User;

                var jwtTokenString = GenerateJwtToken.GenerateJwtTokenString(
                    _coreConfiguration,
                    user.Id.ToString(),
                    user.UserName,
                    user.NameKey,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Roles
                    );

                // Generate refresh token
                var refreshToken = string.Empty;
                try
                {
                    refreshToken = await _mediator.Send(new GenerateRefreshTokenCommand { UserId = user.Id.ToString() });
                }
                catch (Exception ex)
                {
                    Log.Error("There was an error generating a followup refresh token for user:{userId} during authentication {@ex}", user.Id.ToString(), ex);
                }

                return new AuthenticationResponse { isSuccess = true, JwtToken = jwtTokenString, RefreshToken = refreshToken, User = user, Message = "Authentication succeeded" };
            }

        }

    }
}
