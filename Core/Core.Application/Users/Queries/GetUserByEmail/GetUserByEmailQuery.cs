using Core.Application.Users.Models.Views;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Queries.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<User>
    {
        public string Email { get; set; }
    }
}
