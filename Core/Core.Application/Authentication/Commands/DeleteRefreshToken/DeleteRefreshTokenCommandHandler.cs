using Core.Common.Response;
using Core.Infrastructure.Persistence.DocumentDatabase;
using FluentValidation.Results;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Authentication.Commands.DeleteRefreshToken
{
    public class DeleteInvitationCommandHandler : IRequestHandler<DeleteRefreshTokenCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly IDocumentContext _documentContext;

        public DeleteInvitationCommandHandler(IMediator mediator, IDocumentContext documentContext)
        {
            _mediator = mediator;
            _documentContext = documentContext;
        }

        public async Task<BaseResponse> Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            //=========================================================================
            // VALIDATE OUR COMMAND REQUEST
            //=========================================================================

            DeleteRefreshTokenValidator validator = new DeleteRefreshTokenValidator();
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "Invalid Input" };
            }


            //=========================================================================
            // DELETE DOCUMENT FROM DOCUMENT STORE
            //=========================================================================

            // Generate RequestOptions/ParitionKey
            var requestOptions = new RequestOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.RefreshToken())
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
                Log.Information("Refresh token {id} deleted", request.Id);
                //Log.Information("Invitation deleted {@deletedInvitation}", deletedInvitation);


                return new BaseResponse { isSuccess = true, Message = "Refresh token has been deleted" };
            }
            else
            {
                return new BaseResponse { Message = "Could not delete document. Status code: " + result.StatusCode };
            }
        }

    }
}
