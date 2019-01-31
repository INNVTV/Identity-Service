using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Infrastructure.Pipeline
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        // We run a stopwatch on every request and log a warning for any requests that exceed our threshold.

        private readonly Stopwatch _timer;

        public PerformanceBehavior()
        {
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 2500)
            {
                var name = typeof(TRequest).Name;

                // TODO: Add User/Account/Caller Details, or include in Command
                Log.Warning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@request}", name, _timer.ElapsedMilliseconds, request);

            }

            return response;
        }
    }
}
