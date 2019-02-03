using Core.Application.Invitations.Models.Documents;
using Core.Domain.Entities;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Invitations.Queries.GetInvitationById
{
    public class GetInvitationByIdQueryHandler : IRequestHandler<GetInvitationByIdQuery, Invitation>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public GetInvitationByIdQueryHandler(IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IMediator mediator)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;
        }

        public async Task<Invitation> Handle(GetInvitationByIdQuery request, CancellationToken cancellationToken)
        {
            //==========================================================================
            // PRE QUERY CHECKLIST 
            //==========================================================================
            // 1. CACHING: Check if item exists in cache.
            //     a. Use MediatR, Caching Library or Caching Routine within Accounts
            //
            // NOTE: Redis Multiplexer is already setup in our DI container using IRedisContext
            //--------------------------------------------------------------------------

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
            var result = _documentContext.Client.CreateDocumentQuery<InvitationDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions
            );

            InvitationDocumentModel invitationDocumentModel = null;

            try
            {
                invitationDocumentModel = result.AsEnumerable().FirstOrDefault();
            }
            catch (Exception ex)
            {
                // throw AzureCosmoDBException (if a custom exception type is desired)
                // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                // ...Or pass along as inner exception:
                throw new Exception("An error occured trying to use the document store", ex);
            }

            Invitation invitation = null;

            //==========================================================================
            // POST QUERY CHECKLIST 
            //==========================================================================
            // 1. CACHING: Update results in cache.
            //     a. Use MediatR, Caching Library or Caching Routine within Accounts
            //
            // NOTE: Redis Multiplexer is already setup in our DI container using IRedisContext
            //--------------------------------------------------------------------------

            if (invitationDocumentModel != null)
            {
                //Use AutoMapper to transform DocumentModel into Domain Model (Configure via Core.Startup.AutoMapperConfiguration)
                invitation = AutoMapper.Mapper.Map<Core.Domain.Entities.Invitation>(invitationDocumentModel);


                // Assign expiration details
                invitation.DaysSinceCreated = (DateTime.UtcNow - invitation.CreatedDate).Days;

                if (invitation.DaysSinceCreated >= _coreConfiguration.Invitations.ExpirationDays)
                {
                    invitation.Expired = true;
                }
            }

            return invitation;
        }
    }
}
