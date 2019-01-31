﻿using Core.Application.Users.Models.Documents;
using Core.Application.Users.Models.Views;
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

namespace Core.Application.Users.Queries.GetUserByEmail
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDetailsViewModel>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IDocumentContext _documentContext;

        public GetUserByEmailQueryHandler(IDocumentContext documentContext, ICoreConfiguration coreConfiguration, IMediator mediator)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _documentContext = documentContext;

            //Log Activity, Check Authorization, Etc...
        }

        public async Task<UserDetailsViewModel> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
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
            string sqlQuery = "SELECT * FROM Documents d WHERE d.Email ='" + request.Email.ToLower() + "'";
            var sqlSpec = new SqlQuerySpec { QueryText = sqlQuery };

            // Generate collection uri
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

            // Generate FeedOptions/ParitionKey
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(Common.Constants.DocumentType.User())
            };

            // Run query against the document store
            var result = _documentContext.Client.CreateDocumentQuery<UserDocumentModel>(
                collectionUri,
                sqlSpec,
                feedOptions 
            );

            UserDocumentModel userDocumentModel;

            try
            {
                userDocumentModel = result.AsEnumerable().FirstOrDefault();
            }
            catch (Exception ex)
            {
                // throw AzureCosmoDBException (if a custom exception type is desired)
                // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                // ...Or pass along as inner exception:
                throw new Exception("An error occured trying to use the document store", ex);
            }

            // Create our ViewModel and transform our document model
            var userViewModel = new UserDetailsViewModel();

            //==========================================================================
            // POST QUERY CHECKLIST 
            //==========================================================================
            // 1. CACHING: Update results in cache.
            //     a. Use MediatR, Caching Library or Caching Routine within Accounts
            //
            // NOTE: Redis Multiplexer is already setup in our DI container using IRedisContext
            //--------------------------------------------------------------------------

            // TODO: Check user role to include data for the view to use
            userViewModel.DeleteEnabled = true;
            userViewModel.EditEnabled = true;

            if(userDocumentModel != null)
            {
                //Use AutoMapper to transform DocumentModel into Domain Model (Configure via Core.Startup.AutoMapperConfiguration)
                var user = AutoMapper.Mapper.Map<Core.Domain.Entities.User>(userDocumentModel);
                userViewModel.User = user;
            }

            return userViewModel;
            
        }
    }
}
