using Core.Application.Users.Models.Views;
using MediatR;

namespace Core.Application.Users.Queries.GetUserByUserName
{
    public class GetUserByUserNameQuery : IRequest<UserDetailsViewModel>
    {
        public string UserName { get; set; }
    }
}
