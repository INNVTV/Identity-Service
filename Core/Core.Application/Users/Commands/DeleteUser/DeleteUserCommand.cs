using Core.Common.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }
}
