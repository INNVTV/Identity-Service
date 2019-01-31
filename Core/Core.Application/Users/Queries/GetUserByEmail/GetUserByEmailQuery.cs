using Core.Application.Users.Models.Views;
using MediatR;

namespace Core.Application.Users.Queries.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<UserDetailsViewModel>
    {
        public string Email { get; set; }
    }
}
