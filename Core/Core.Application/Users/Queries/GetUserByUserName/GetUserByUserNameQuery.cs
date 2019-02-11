using Core.Application.Users.Models.Views;
using MediatR;

namespace Core.Application.Users.Queries.GetUserByUserName
{
    public class GetUserByUserNameQuery : IRequest<Domain.Entities.User>
    {
        public string UserName { get; set; }
    }
}
