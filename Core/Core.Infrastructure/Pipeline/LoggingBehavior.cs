using Core.Domain.Entities;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Infrastructure.Pipeline
{
    // Unlike Performance and Tracing behavior, this behavior will ONLY run as a pre-processor.
    // Please note the differences in the way the classes are written.

    public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest>
    {
        public LoggingBehavior()
        {
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;

            // TODO: Add User/Caller Details, or include in Command

            //Log.Information("Request: {name} {@request} {@user}", name, request, user);

            return Task.CompletedTask;
        }
    }
}
