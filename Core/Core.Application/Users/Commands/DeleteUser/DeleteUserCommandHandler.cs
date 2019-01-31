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

namespace Core.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;
        private readonly IEmailService _emailService;

        public DeleteUserCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IEmailService emailService)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
            _emailService = emailService;
        }

        public async Task<BaseResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            //=========================================================================
            // VALIDATE OUR COMMAND REQUEST
            //=========================================================================

            DeleteUserValidator validator = new DeleteUserValidator();
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "Invalid Input" };
            }

            //=========================================================================
            // GET USER TO DELETE FROM DOCUMENT STORE
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

            var deletedUser = AutoMapper.Mapper.Map<Core.Domain.Entities.User>(userDocumentModel);


            //=========================================================================
            // DELETE DOCUMENT FROM DOCUMENT STORE
            //=========================================================================

            // Generate RequestOptions/ParitionKey
            var requestOptions = new RequestOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.User())
            };

            // Generate Document Uri
            Uri documentUri = UriFactory.CreateDocumentUri(_documentContext.Settings.Database, _documentContext.Settings.Collection, userDocumentModel.Id);

            // Save the document to document store using the IDocumentContext dependency
            var result = await _documentContext.Client.DeleteDocumentAsync(documentUri, requestOptions);

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                //=========================================================================
                // LOG ACTIVITY
                //=========================================================================
                Log.Information("User deleted {@deletedUser}", deletedUser);

                //=========================================================================
                // SEND EMAIL 
                //=========================================================================
                // Send an email to the account initializer using the IEmailService dependency
                //var emailMessage = new EmailMessage
                //{
                //ToEmail = requestOp,
                //ToName = String.Concat(accountDocumentModel.Owner.FirstName, " ", accountDocumentModel.Owner.LastName),
                //Subject = "User deleted",
                //TextContent = String.Concat("Your account: '", accountDocumentModel.Name, "' has been closed."),
                //HtmlContent = String.Concat("Your account: <b>", accountDocumentModel.Name, "</b> has been closed.")
                //};

                //var emailSent = await _emailService.SendEmail(emailMessage);

                /*=========================================================================
                 * CLEANUP ROUTINES
                 * =========================================================================
                 * 
                 *  1. Delete all document storage partitions associated with this account
                 *  
                 *  2. Delete all storage data associated with this account.
                 *     (blobs, tables and queues)
                 *     
                 *  3. Purge all caches that may still hold data associated with the account
                 *  
                 *  4. Update search index or send indexer request
                 *  
                 *  NOTE: This can be done via message queue picked up by a worker proccess
                 *        or by a record checked by a custodal process
                 *  
                 *  NOTE: The ability to delete an entire document partition is likely coming
                 *        in a future CosmosDB release.
                 *  
                 * --------------------------------------------------------------------------
                 */

                return new BaseResponse { isSuccess = true, Message = "User has been deleted" };
            }
            else
            {
                return new BaseResponse { Message = "Could not delete document. Status code: " + result.StatusCode };
            }

        }
    }
}
