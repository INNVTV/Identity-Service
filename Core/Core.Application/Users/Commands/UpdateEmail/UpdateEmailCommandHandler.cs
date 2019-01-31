using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Models.Documents;
using Core.Common.Response;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using Core.Infrastructure.Services.Email;
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

namespace Core.Application.Users.Commands.UpdateEmail
{
    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;
        private readonly IEmailService _emailService;

        public UpdateEmailCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IEmailService emailService)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
            _emailService = emailService;
        }

        public async Task<BaseResponse> Handle(UpdateEmailCommand request, CancellationToken cancellationToken)
        {

            UpdateEmailValidator validator = new UpdateEmailValidator(_mediator); //, _coreConfiguration);
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "One or more validation errors occurred." };
            }

            //=========================================================================
            // GET the USER DOCUMENT MODEL
            //=========================================================================

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(
                _documentContext.Settings.Database, 
                _documentContext.Settings.Collection);


            // Create the query
            string sqlQuery = "SELECT * FROM Documents d WHERE d.id ='" + request.id + "'";

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
            var oldEmail = String.Empty; // For logging

            if (userDocumentModel == null)
            {
                return new BaseResponse { Message = "Could not retrieve user with that Id from the document store." };
            }
            else
            {
                oldEmail = userDocumentModel.Email;
                userDocumentModel.Email = request.NewEmail.ToLower().Trim();
            }

            
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
                Log.Information("Email updated from {oldEmail} to {newName} {@user}", oldEmail, request.NewEmail, user);


                //==========================================================================
                // POST COMMAND CHECKLIST 
                //=========================================================================
                // 1. CACHING: Update cache.
                // 2. SEARCH INDEX: Update Search index or send indexer request.
                //-----------------------------------------------------------------------

                return new BaseResponse { isSuccess = true, Message = "Email updated." };
            }
            else
            {
                return new BaseResponse { Message = "Could not save model to document store. Status code: " + result.StatusCode };
            }
        }
    }
}
