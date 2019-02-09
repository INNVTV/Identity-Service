using Core.Application.Authentication.Models.Documents;
using Core.Infrastructure.Persistence.DocumentDatabase;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Authentication.Commands.GenerateRefreshToken
{
    public class GenerateRefreshTokenCommandHandler :IRequestHandler<GenerateRefreshTokenCommand, string>
    {
        IDocumentContext _documentContext;

        public GenerateRefreshTokenCommandHandler(IDocumentContext documentContext)
        {
            _documentContext = documentContext;
        }

        public async Task<string> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenString = String.Empty;

            if(!String.IsNullOrEmpty(request.UserId))
            {
                //=========================================================================
                // CREATE AND STORE OUR DOCUMENT MODEL
                //=========================================================================

                var refreshTokenDocumentModel = new RefreshTokenDocumentModel(request.UserId);

                // Generate collection uri
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_documentContext.Settings.Database, _documentContext.Settings.Collection);

                ResourceResponse<Document> result;

                // Save the document to document store using the IDocumentContext dependency
                result = await _documentContext.Client.CreateDocumentAsync(
                    collectionUri,
                    refreshTokenDocumentModel
                );

                tokenString = refreshTokenDocumentModel.Id;
            }

            return tokenString;
        }
     
    }    
}
