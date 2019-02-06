using Core.Application.Passwords.Models;
using Core.Application.Users.Queries.GetUserById;
using Core.Infrastructure.Persistence.RedisCache;
using MediatR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Passwords.Queries.GetResetRequest
{
    public class GetResetRequestQueryHandler : IRequestHandler<GetResetRequestQuery, ResetRequestModel>
    {
        private readonly IMediator _mediator;
        private readonly IRedisContext _redisContext;
        

        public GetResetRequestQueryHandler(IMediator mediator, IRedisContext redisContext)
        {
            _mediator = mediator;
            _redisContext = redisContext;
        }

        public async Task<ResetRequestModel> Handle(GetResetRequestQuery request, CancellationToken cancellationToken)
        {
            if(String.IsNullOrEmpty(request.ResetCode))
            {
                return new ResetRequestModel { };
            }
            else
            {
                try
                {
                    #region Setup our caching client and key

                    IDatabase cache = _redisContext.ConnectionMultiplexer.GetDatabase();
                    var resetCodeCacheKey = Common.Constants.CachingKeys.PasswordResetCode(request.ResetCode);

                    #endregion

                    #region Check if exists

                    var resetCodeCacheValue = cache.StringGet(resetCodeCacheKey);

                    if (resetCodeCacheValue.HasValue)
                    {
                        var userId = resetCodeCacheValue;
                        var userViewModel = await _mediator.Send(new GetUserByIdQuery { Id = userId });

                        return new ResetRequestModel(request.ResetCode, userViewModel.User);

                    }
                    else
                    {
                        return new ResetRequestModel { };
                    }
                    #endregion

                }
                catch
                {

                }
            }
            return new ResetRequestModel { };
        }
    }
}
