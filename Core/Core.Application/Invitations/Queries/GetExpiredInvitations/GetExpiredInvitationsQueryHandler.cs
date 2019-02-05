using Core.Application.Invitations.Models;
using Core.Application.Invitations.Models.Documents;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Invitations.Queries.GetExpiredInvitations
{
    public class GetExpiredInvitationsQueryHandler: IRequestHandler<GetExpiredInvitationsQuery, ExpiredInvitationsModel>
    {
        //MediatR will automatically inject  dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;


        public GetExpiredInvitationsQueryHandler(IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IMediator mediator)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;

        }

        public async Task<ExpiredInvitationsModel> Handle(GetExpiredInvitationsQuery request, CancellationToken cancellationToken)
        {
            //-----------------------------------------------------
            // TODO: DocumentDB will soon have skip/take
            // For now we use continuation token
            // For more robust query capabilities use Azure Search via: SearchAccountsQuery
            //-----------------------------------------------------

            // Prepare our view model to be returned
            var expiredInvitationsModel = new ExpiredInvitationsModel();

            // Convert expired days to negative number
            var expiredDaysSubtraction = System.Math.Abs(_coreConfiguration.Invitations.ExpirationDays) * (-1);

            //var sqlQuery = new StringBuilder(String.Concat(
                //"SELECT d.id FROM Documents d WHERE d.CreatedDate <= ", DateTime.UtcNow.AddDays(expiredDaysSubtraction)));

            //var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery.ToString() };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.Invitation()),
                MaxItemCount = request.PageSize, //<-- This is the page size
                RequestContinuation = request.ContinuationToken
            };

            try
            {
                // Create the document query
                var query = _documentContext.Client.CreateDocumentQuery<InvitationDocumentModel>(
                    collectionUri,
                    //sqlSpec,
                    feedOptions
                )
                .Where(d => d. CreatedDate <= DateTime.UtcNow.AddDays(expiredDaysSubtraction))
                .AsDocumentQuery(); //<-- 'AsDocumentQuery' extension method casts the IOrderedQueryable query to an IDocumentQuery

                // Run query against the document store
                var result = await query.ExecuteNextAsync<InvitationDocumentModel>(); //<-- Get the first page of results as AccountDocumentModel(s)

                if (query.HasMoreResults)
                {
                    //If there are more results pass back a continuation token so the caller can get the next batch
                    expiredInvitationsModel.HasMoreResults = true;
                    expiredInvitationsModel.ContinuationToken = result.ResponseContinuation;
                }

                if (result != null && result.Count > 0)
                {
                    expiredInvitationsModel.Count = result.Count;

                    foreach (var id in result)
                    {
                        expiredInvitationsModel.Ids.Add(id.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Warning("There was an issue accessing the document store {@ex}", ex);
            }


            return expiredInvitationsModel;


        }
    }
}
