using Core.Application.Users.Models.Views;
using MediatR;

namespace Core.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDetailsViewModel>
    {
        public string Id { get; set; }
    }
}
