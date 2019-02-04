using Core.Application.Invitations.Models.Documents;
using Core.Common.Response;
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

namespace Core.Application.Invitations.Commands.DeleteInvitation
{
    public class DeleteInvitationCommandHandler : IRequestHandler<DeleteInvitationCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly IDocumentContext _documentContext;

        public DeleteInvitationCommandHandler(IMediator mediator, IDocumentContext documentContext)
        {
            _mediator = mediator;
            _documentContext = documentContext;
        }

        public async Task<BaseResponse> Handle(DeleteInvitationCommand request, CancellationToken cancellationToken)
        {
            //=========================================================================
            // VALIDATE OUR COMMAND REQUEST
            //=========================================================================

            DeleteInvitationValidator validator = new DeleteInvitationValidator();
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "Invalid Input" };
            }

            //=========================================================================
            // GET INVITATION TO DELETE FROM DOCUMENT STORE
            //=========================================================================
            /*
            // Create the query
            string sqlQuery = "SELECT * FROM Documents d WHERE d.id ='" + request.Id + "'";
            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.Invitation())
            };

            // Run query against the document store
            var getResult = _documentContext.Client.CreateDocumentQuery<InvitationDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions
            );

            var invitationDocumentModel = getResult.AsEnumerable().FirstOrDefault();

            if (invitationDocumentModel == null)
            {
                return new BaseResponse { Message = "Invitation does not exist." };
            }

            var deletedInvitation = AutoMapper.Mapper.Map<Core.Domain.Entities.Invitation>(invitationDocumentModel);
            */

            //=========================================================================
            // DELETE DOCUMENT FROM DOCUMENT STORE
            //=========================================================================

            // Generate RequestOptions/ParitionKey
            var requestOptions = new RequestOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.Invitation())
            };

            // Generate Document Uri
            Uri documentUri = UriFactory.CreateDocumentUri(_documentContext.Settings.Database, _documentContext.Settings.Collection, request.Id);

            // Save the document to document store using the IDocumentContext dependency
            var result = await _documentContext.Client.DeleteDocumentAsync(documentUri, requestOptions);

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                //=========================================================================
                // LOG ACTIVITY
                //=========================================================================
                Log.Information("Invitation {id} deleted", request.Id);
                //Log.Information("Invitation deleted {@deletedInvitation}", deletedInvitation);

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

                return new BaseResponse { isSuccess = true, Message = "Invitation has been deleted" };
            }
            else
            {
                return new BaseResponse { Message = "Could not delete document. Status code: " + result.StatusCode };
            }
        }

    }
}
