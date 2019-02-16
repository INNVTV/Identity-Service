using Core.Application.Invitations.Models.Documents;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Invitations.Commands.InviteUser
{
    public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;
        private readonly IEmailService _emailService;

        public InviteUserCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IEmailService emailService)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
            _emailService = emailService;
        }

        public async Task<BaseResponse> Handle(InviteUserCommand request, CancellationToken cancellationToken)
        {
            //=========================================================================
            // VALIDATE OUR COMMAND REQUEST USING FLUENT VALIDATION 
            // ValidationExceptions can be captured using Middleware. However it is not as testable or portable outside of the MVC framework
            // I prefer using manual/granular validation within each command. 
            //=========================================================================

            InviteUserValidator validator = new InviteUserValidator(_mediator); //, _coreConfiguration);
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "One or more validation errors occurred" };
            }


            //=========================================================================
            // CREATE AND STORE OUR DOCUMENT MODEL
            //=========================================================================

            var invitationDocumentModel = new InvitationDocumentModel(request.Email, request.Roles);

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            ResourceResponse<Document> result;

            try
            {
                // Save the document to document store using the IDocumentContext dependency
                result = await _documentContext.Client.CreateDocumentAsync(
                    collectionUri,
                    invitationDocumentModel
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


                //=========================================================================
                // SEND EMAIL 
                //=========================================================================
                // Send an email to the new user using the IEmailService dependency
                var invitationLink = String.Concat(_coreConfiguration.Endpoints.ClientDomain, _coreConfiguration.Endpoints.AcceptInvitationsPath, invitationDocumentModel.Id);

                var emailMessage = new EmailMessage
                {
                    ToEmail = request.Email,
                    //ToName = String.Concat(""),
                    Subject = String.Concat("Invitation to ", _coreConfiguration.Application.Name),
                    TextContent = String.Concat("You have been invited to ", _coreConfiguration.Application.Name, ". Please accept your invite and create your identity by following the instructions at this link: ", invitationLink),
                    HtmlContent = String.Concat("You have been invited to ", _coreConfiguration.Application.Name, ".</br></br> Please accept your invite and create your identity by following the instructions <a href='", invitationLink, "'>here</a>"),
                };

                try
                {
                    var emailSent = await _emailService.SendEmail(emailMessage);
                }
                catch (Exception ex)
                {
                    // throw EmailServiceException (if a custom exception type is desired)
                    // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                    // Pass the exception up the chain:
                    throw ex;

                    // ...Or pass along as inner exception:
                    //throw new Exception("An error occured trying to use the email service", ex);
                }
                finally
                {
                    // Use alternate communication method...
                }

                //=========================================================================
                // LOG ACTIVITY
                //=========================================================================
                Log.Information("Invitation created {@invitation}", invitationDocumentModel);

                //==========================================================================
                // POST COMMAND CHECKLIST 
                //=========================================================================
                // 1. CACHING: Update cache.
                // 2. SEARCH INDEX: Update Search index or send indexer request.
                //-----------------------------------------------------------------------

                return new BaseResponse { isSuccess = true, Message = "Invitation sent" };
            }
            else
            {
                return new BaseResponse { Message = "Could not save model to document store. Status code: " + result.StatusCode };
            }
        }
    }
}
