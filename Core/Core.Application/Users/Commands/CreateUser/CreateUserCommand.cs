using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<CreateUserCommandResponse>
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<string> Roles { get; set; }
    }
}
