using Core.Application.Users.Models.Documents;
using Core.Application.Users.Models.Views;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Serilog;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Users.Queries.GetUsersList
{
    public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, UsersListResultsViewModel>
    {
        //MediatR will automatically inject  dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;


        public GetUsersListQueryHandler(IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IMediator mediator)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;

        }

        public async Task<UsersListResultsViewModel> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
        {
            //-----------------------------------------------------
            // TODO: DocumentDB will soon have skip/take
            // For now we use continuation token
            // For more robust query capabilities use Azure Search via: SearchAccountsQuery
            //-----------------------------------------------------

            // Prepare our view model to be returned
            var usersListViewModel = new UsersListResultsViewModel();

            // TODO: Check user role to include data for the view to use
            usersListViewModel.DeleteEnabled = false;
            usersListViewModel.EditEnabled = false;

            // Create the query
            // NOTE: we are specifying properties to minimize query size

            var sqlQuery = new StringBuilder(String.Concat(
                "SELECT d.id, d.UserName, d.NameKey, d.CreatedDate FROM Documents d ORDER BY d.",
                request.OrderBy,
                " ",
                request.OrderDirection
                ));

            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery.ToString() };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.PlatformUser()),
                MaxItemCount = request.PageSize, //<-- This is the page size
                RequestContinuation = request.ContinuationToken
            };

            try
            {
                // Create the document query
                var query = _documentContext.Client.CreateDocumentQuery<UserDocumentModel>(
                    collectionUri,
                    sqlSpec,
                    feedOptions
                ).AsDocumentQuery(); //<-- 'AsDocumentQuery' extension method casts the IOrderedQueryable query to an IDocumentQuery

                // Run query against the document store
                var result = await query.ExecuteNextAsync<UserDocumentModel>(); //<-- Get the first page of results as AccountDocumentModel(s)

                if (query.HasMoreResults)
                {
                    //If there are more results pass back a continuation token so the caller can get the next batch
                    usersListViewModel.HasMoreResults = true;
                    usersListViewModel.ContinuationToken = result.ResponseContinuation;
                }

                if (result != null && result.Count > 0)
                {
                    usersListViewModel.Count = result.Count;

                    foreach (var accountDocument in result)
                    {
                        //Use AutoMapper to transform DocumentModel into Domain Model (Configure via Core.Startup.AutoMapperConfiguration)
                        var user = AutoMapper.Mapper.Map<UserListViewItem>(accountDocument);
                        usersListViewModel.Users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Warning("There was an issue accessing the document store {@ex}", ex);
            }


            return usersListViewModel;


        }
    }
}
