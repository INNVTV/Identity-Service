using Core.Application.Users.Models.Documents;
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

namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(IMediator mediator, IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IEmailService emailService)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
            _emailService = emailService;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            //=========================================================================
            // VALIDATE OUR COMMAND REQUEST USING FLUENT VALIDATION 
            // ValidationExceptions can be captured using Middleware. However it is not as testable or portable outside of the MVC framework
            // I prefer using manual/granular validation within each command. 
            //=========================================================================

            CreateUserValidator validator = new CreateUserValidator(_mediator); //, _coreConfiguration);
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new CreateUserCommandResponse(validationResult.Errors) { Message = "One or more validation errors occurred" };
            }



            //=========================================================================
            // CREATE AND STORE OUR DOCUMENT MODEL
            //=========================================================================

            // Generate salt and hash from password
            var passwordHashResults = Common.Encryption.PasswordHashing.HashPassword(request.Password);

            var userDocumentModel = new UserDocumentModel(request.FirstName, request.LastName, request.UserName, request.Email, request.Roles, passwordHashResults.Salt, passwordHashResults.Hash);

            // Create the initial user/owner
            //accountDocumentModel.Owner.Email = request.Email;
            //accountDocumentModel.Owner.FirstName = request.FirstName;
            //accountDocumentModel.Owner.LastName = request.LastName;

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
                    userDocumentModel
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
                var emailMessage = new EmailMessage
                {
                    ToEmail = request.Email,
                    ToName = String.Concat(request.FirstName, " ", request.LastName),
                    Subject = "User created",
                    TextContent = String.Concat("Thank you ", String.Concat(request.FirstName, " ", request.LastName), "! your user identity has been created!"),
                    HtmlContent = String.Concat("Thank you ", String.Concat(request.FirstName, " ", request.LastName), ",<br>Your user identity has been created!")
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

                //==========================================================================
                // AUTOMAPPER 
                //=========================================================================
                // Create our domain model using AutoMapper to be returned within our response object.
                // Add additional mappings into the: Core.Startup.AutoMapperConfiguration class.

                var user = AutoMapper.Mapper.Map<Core.Domain.Entities.User>(userDocumentModel);

                //=========================================================================
                // LOG ACTIVITY
                //=========================================================================
                Log.Information("User created {@user}", user);

                //==========================================================================
                // POST COMMAND CHECKLIST 
                //=========================================================================
                // 1. CACHING: Update cache.
                // 2. SEARCH INDEX: Update Search index or send indexer request.
                //-----------------------------------------------------------------------

                return new CreateUserCommandResponse { isSuccess = true, User = user, Message = "User created" };
            }
            else
            {
                return new CreateUserCommandResponse { Message = "Could not save model to document store. Status code: " + result.StatusCode };
            }
        }
    }
}
