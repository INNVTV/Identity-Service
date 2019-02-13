using Core.Application.Passwords.Queries.GetResetRequest;
using Core.Application.Users.Models.Documents;
using Core.Common.Response;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using Core.Infrastructure.Persistence.RedisCache;
using FluentValidation.Results;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Passwords.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;
        private readonly IRedisContext _redisContext;


        public ResetPasswordCommandHandler(IMediator mediator, ICoreConfiguration coreConfiguration, IRedisContext redisContext, IDocumentContext documentContext)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _redisContext = redisContext;
            _documentContext = documentContext;
        }

        public async Task<BaseResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            ResetPasswordValidator validator = new ResetPasswordValidator();
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "One or more validation errors occurred" };
            }

            //=========================================================================
            // VERFIY THE REQUEST CODE IS VALID
            //=========================================================================
            var resetRequest = await _mediator.Send(new GetResetRequestQuery { ResetCode = request.ResetCode });
            if(!resetRequest.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "Reset request is not valid" };
            }


            //=========================================================================
            // GET the USER DOCUMENT MODEL
            //=========================================================================

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                _documentContext.Settings.Database,
                _documentContext.Settings.Collection);


            // Create the query
            string sqlQuery = "SELECT * FROM Documents d WHERE d.id ='" + request.UserId + "'";

            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.User())
            };

            // Run query against the document store
            var queryResult = _documentContext.Client.CreateDocumentQuery<UserDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions
            );

            UserDocumentModel userDocumentModel;

            try
            {
                userDocumentModel = queryResult.AsEnumerable().FirstOrDefault();
            }
            catch (Exception ex)
            {
                // throw AzureCosmoDBException (if a custom exception type is desired)
                // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                // ...Or pass along as inner exception:
                throw new Exception("An error occured trying to use the document store", ex);
            }

            //=========================================================================
            // UPDATE the USER DOCUMENT MODEL
            //=========================================================================


            if (userDocumentModel == null)
            {
                return new BaseResponse { Message = "Could not retrieve user with that Id from the document store" };
            }



            // Generate salt and hash from new password
            var passwordHashResults = Common.Encryption.PasswordHashing.HashPassword(request.NewPassword);

            userDocumentModel.PasswordSalt = passwordHashResults.Salt;
            userDocumentModel.PasswordHash = passwordHashResults.Hash;


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
                    new RequestOptions { PartitionKey = new PartitionKey(Common.Constants.DocumentType.User().ToString()) }
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


            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //=========================================================================
                // LOG ACTIVITY
                //=========================================================================
                var user = AutoMapper.Mapper.Map<Core.Domain.Entities.User>(userDocumentModel);
                Log.Information("Password reset {@user}", user);


                //==========================================================================
                // POST COMMAND CHECKLIST 
                //=========================================================================
                // 1. CACHING: Update cache.
                // 2. SEARCH INDEX: Update Search index or send indexer request.
                //-----------------------------------------------------------------------

                // Clear redis cach record
                #region Clear cached reset request (Redis Cache)

                IDatabase cache = _redisContext.ConnectionMultiplexer.GetDatabase();
                var resetCodeCacheKey = Common.Constants.CachingKeys.PasswordResetCode(request.ResetCode);
                cache.KeyDelete(resetCodeCacheKey, CommandFlags.FireAndForget);

                #endregion

                // Return Response
                return new BaseResponse { isSuccess = true, Message = "Password reset" };
            }
            else
            {
                return new BaseResponse { Message = "Could not save model to document store. Status code: " + result.StatusCode };
            }













        }
    }
}
