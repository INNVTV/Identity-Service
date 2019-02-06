using Core.Application.Passwords.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Queries.GetResetRequest
{
    public class GetResetRequestQuery: IRequest<ResetRequestModel>
    {
        public string ResetCode { get; set; }
    }
}
