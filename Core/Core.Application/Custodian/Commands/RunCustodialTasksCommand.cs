using Core.Application.Custodian.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Custodian.Commands
{
    public class RunCustodialTasksCommand : IRequest<CustodialReport>
    {
    }
}
