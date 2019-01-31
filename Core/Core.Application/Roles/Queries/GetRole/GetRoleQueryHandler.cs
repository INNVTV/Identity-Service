using Core.Application.Roles.Models.Documents;
using Core.Application.Roles.Models.Views;
using Core.Application.Roles.Queries.GetRole;
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

namespace Core.Application.Roles.Queries.GetRole
{
    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, RoleViewModel>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public GetRoleQueryHandler(IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IMediator mediator)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;

            //Log Activity, Check Authorization, Etc...
        }

        public async Task<RoleViewModel> Handle(GetRoleQuery request, CancellationToken cancellationToken)
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
            string sqlQuery = "SELECT * FROM Documents d WHERE d.NameKey ='" + Common.Transformations.NameKey.Transform(request.NameKey) + "'";
            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.Role())
            };

            // Run query against the document store
            var result = _documentContext.Client.CreateDocumentQuery<RoleDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions
            );

            RoleDocumentModel roleDocumentModel;

            try
            {
                roleDocumentModel = result.AsEnumerable().FirstOrDefault();
            }
            catch (Exception ex)
            {
                // throw AzureCosmoDBException (if a custom exception type is desired)
                // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                // ...Or pass along as inner exception:
                throw new Exception("An error occured trying to use the document store", ex);
            }

            // Create our ViewModel and transform our document model
            var roleViewModel = new RoleViewModel();

            //==========================================================================
            // POST QUERY CHECKLIST 
            //==========================================================================
            // 1. CACHING: Update results in cache.
            //     a. Use MediatR, Caching Library or Caching Routine within Accounts
            //
            // NOTE: Redis Multiplexer is already setup in our DI container using IRedisContext
            //--------------------------------------------------------------------------


            if (roleDocumentModel != null)
            {
                //Use AutoMapper to transform DocumentModel into Domain Model (Configure via Core.Startup.AutoMapperConfiguration)
                var role = AutoMapper.Mapper.Map<Role>(roleDocumentModel);
                roleViewModel.Role = role;
            }

            return roleViewModel;

        }
    }
}
