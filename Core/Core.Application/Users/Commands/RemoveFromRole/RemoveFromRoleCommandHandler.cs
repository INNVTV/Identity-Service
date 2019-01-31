using Core.Application.Roles.Queries.GetRoles;
using Core.Application.Users.Models.Documents;
using Core.Common.Response;
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

namespace Core.Application.Users.Commands.RemoveFromRole
{
    public class RemoveFromRoleCommandHandler : IRequestHandler<RemoveFromRoleCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public RemoveFromRoleCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
        }

        public async Task<BaseResponse> Handle(RemoveFromRoleCommand request, CancellationToken cancellationToken)
        {

            // Validate input
            RemoveFromRoleValidator validator = new RemoveFromRoleValidator(_mediator);
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "One or more validation errors occurred." };
            }

            //=========================================================================
            // GET USER FROM DOCUMENT STORE
            //=========================================================================

            // Create the query
            string sqlQuery = "SELECT * FROM Documents d WHERE d.id ='" + request.Id + "'";
            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.User())
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
                return new BaseResponse { Message = "User does not exist." };
            }

            // Make sure user is in the role first 
            if (!userDocumentModel.Roles.Contains(request.RoleToRemove))
            {
                return new BaseResponse { Message = "User is not assigned to this role" };
            }


            //=========================================================================
            // REMOVE THE ROLE AND UPDATE THE DOCUMENT
            //=========================================================================

            userDocumentModel.Roles.Remove(request.RoleToRemove);

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
                Log.Information("{@user} removed from the role: {role}", user, request.RoleToRemove);

                //==========================================================================
                // POST COMMAND CHECKLIST 
                //=========================================================================
                // 1. CACHING: Update cache.
                // 2. SEARCH INDEX: Update Search index or send indexer request.
                //-----------------------------------------------------------------------

                return new BaseResponse { isSuccess = true, Message = "User removed from role" };
            }
            else
            {
                return new BaseResponse { Message = "Could not save model to document store. Status code: " + result.StatusCode };
            }
        }
    }
}
