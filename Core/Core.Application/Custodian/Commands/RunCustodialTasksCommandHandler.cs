using Core.Application.Custodian.Models;
using Core.Application.Invitations.Commands.DeleteInvitation;
using Core.Application.Invitations.Queries.GetExpiredInvitations;
using Core.Infrastructure.Configuration;
using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Custodian.Commands
{
    public class RunCustodialTasksCommandHandler : IRequestHandler<RunCustodialTasksCommand, CustodialReport>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;

        public RunCustodialTasksCommandHandler(IMediator mediator, ICoreConfiguration coreConfiguration)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
        }

        public async Task<CustodialReport> Handle(RunCustodialTasksCommand request, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            var report = new CustodialReport();

            #region Delete Expired Invitations

            var expiredInvitationsQuery = new GetExpiredInvitationsQuery();

            var expiredInventoryCount = 0;
            var processNextBatch = true;

            while (processNextBatch)
            {
                var result = await _mediator.Send(expiredInvitationsQuery);

                if (result.Count > 0)
                {
                    expiredInventoryCount = expiredInventoryCount + result.Count;

                    foreach (var id in result.Ids)
                    {
                        var deleteInvitationCommand = new DeleteInvitationCommand { Id = id };
                        var deleteResult = await _mediator.Send(deleteInvitationCommand);
                        report.TaskLog.Add(String.Concat("Expired invitation deleted. Id: ", id));
                    }
                    processNextBatch = result.HasMoreResults;
                }
                else
                {
                    processNextBatch = false;
                }
            }

            #endregion

            timer.Stop();

            report.TaskCount = report.TaskLog.Count;
            report.isSuccess = true;      
            report.Duration = timer.ElapsedMilliseconds;
            report.Message = String.Concat("Tasks completed in ", timer.ElapsedMilliseconds, " milliseconds");

            Log.Information("Custodial tasks completed {@report}", report);

            return report;
        }
    }
}
