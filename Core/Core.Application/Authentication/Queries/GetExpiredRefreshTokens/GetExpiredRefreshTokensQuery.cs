using Core.Application.Authentication.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Authentication.Queries.GetExpiredRefreshTokens
{
    public class GetExpiredRefreshTokensQuery : IRequest<ExpiredRefreshTokensModel>
    {
        public GetExpiredRefreshTokensQuery()
        {
            //Default Query Options
            PageSize = 20;
            ContinuationToken = null;
        }

        public int PageSize { get; set; }
        public string ContinuationToken; //<-- Null on first call
    }
}
