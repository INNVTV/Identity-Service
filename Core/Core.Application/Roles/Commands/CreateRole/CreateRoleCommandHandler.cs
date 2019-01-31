using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FluentValidation.Results;
using Core.Application.Roles.Models.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Core.Domain.Entities;
using System.Threading.Tasks;
using Serilog;

namespace Core.Application.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CreateRoleCommandResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public CreateRoleCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
        }

        public async Task<CreateRoleCommandResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {

            //=========================================================================
            // VALIDATE OUR COMMAND REQUEST USING FLUENT VALIDATION 
            // ValidationExceptions can be captured using Middleware. However it is not as testable or portable outside of the MVC framework
            // I prefer using manual/granular validation within each command. 
            //=========================================================================


            CreateRoleValidator validator = new CreateRoleValidator(_mediator);
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new CreateRoleCommandResponse(validationResult.Errors) { Message = "One or more validation errors occurred." };
            }



            //=========================================================================
            // CREATE AND STORE OUR DOCUMENT MODEL
            //=========================================================================


            // Create the Document Model
            var roleDocumentModel = new RoleDocumentModel(request.Name, request.Description);


            // Add the ParitionKey to the document (Moved to document constructor)
            //accountDocumentModel.DocumentType = Common.Constants.DocumentType.Account();

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            ResourceResponse<Document> result;

            try
            {
                // Save the document to document store using the IDocumentContext dependency
                result = await _documentContext.Client.CreateDocumentAsync(
                    collectionUri,
                    roleDocumentModel
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


            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                //==========================================================================
                // AUTOMAPPER 
                //=========================================================================
                // Create our domain model using AutoMapper to be returned within our response object.
                // Add additional mappings into the: Core.Startup.AutoMapperConfiguration class.

                var role = AutoMapper.Mapper.Map<Role>(roleDocumentModel);

                //=========================================================================
                // LOG ACTIVITY
                //=========================================================================
                Log.Information("Added new role {@role}", role);

                //==========================================================================
                // POST COMMAND CHECKLIST 
                //=========================================================================
                // 1. CACHING: Update cache.
                // 2. SEARCH INDEX: Update Search index or send indexer request.
                //-----------------------------------------------------------------------

                return new CreateRoleCommandResponse { isSuccess = true, Role = role, Message = "Role created." };
            }
            else
            {
                return new CreateRoleCommandResponse { Message = "Could not save model to document store. Status code: " + result.StatusCode };
            }
        }
    }
}
