using Core.Application.Authentication.Models;
using Core.Application.Authentication.Models.Documents;
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

namespace Core.Application.Authentication.Queries.GetExpiredRefreshTokens
{
    public class GetExpiredRefreshTokensQueryHandler : IRequestHandler<GetExpiredRefreshTokensQuery, ExpiredRefreshTokensModel>
    {
        //MediatR will automatically inject  dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;


        public GetExpiredRefreshTokensQueryHandler(IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IMediator mediator)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;

        }

        public async Task<ExpiredRefreshTokensModel> Handle(GetExpiredRefreshTokensQuery request, CancellationToken cancellationToken)
        {
            //-----------------------------------------------------
            // TODO: DocumentDB will soon have skip/take
            // For now we use continuation token
            // For more robust query capabilities use Azure Search via: SearchAccountsQuery
            //-----------------------------------------------------

            // Prepare our view model to be returned
            var expiredRefreshTokensModel = new ExpiredRefreshTokensModel();

            // Convert expired days to negative number
            var expiredHoursSubtraction = System.Math.Abs(_coreConfiguration.JSONWebTokens.RefreshExpirationHours) * (-1);

            //var sqlQuery = new StringBuilder(String.Concat(
            //"SELECT d.id FROM Documents d WHERE d.CreatedDate <= ", DateTime.UtcNow.AddDays(expiredDaysSubtraction)));

            //var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery.ToString() };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.RefreshToken()),
                MaxItemCount = request.PageSize, //<-- This is the page size
                RequestContinuation = request.ContinuationToken
            };

            try
            {
                // Create the document query
                var query = _documentContext.Client.CreateDocumentQuery<RefreshTokenDocumentModel>(
                    collectionUri,
                    //sqlSpec,
                    feedOptions
                )
                .Where(d => d.CreatedDate <= DateTime.UtcNow.AddHours(expiredHoursSubtraction))
                .AsDocumentQuery(); //<-- 'AsDocumentQuery' extension method casts the IOrderedQueryable query to an IDocumentQuery

                // Run query against the document store
                var result = await query.ExecuteNextAsync<RefreshTokenDocumentModel>(); //<-- Get the first page of results as AccountDocumentModel(s)

                if (query.HasMoreResults)
                {
                    //If there are more results pass back a continuation token so the caller can get the next batch
                    expiredRefreshTokensModel.HasMoreResults = true;
                    expiredRefreshTokensModel.ContinuationToken = result.ResponseContinuation;
                }

                if (result != null && result.Count > 0)
                {
                    expiredRefreshTokensModel.Count = result.Count;

                    foreach (var id in result)
                    {
                        expiredRefreshTokensModel.Ids.Add(id.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Warning("There was an issue accessing the document store {@ex}", ex);
            }
            return expiredRefreshTokensModel;

        }
    }
}
