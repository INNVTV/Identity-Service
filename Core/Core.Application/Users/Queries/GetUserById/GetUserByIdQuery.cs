using Core.Application.Users.Models.Views;
using MediatR;

namespace Core.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<Domain.Entities.User>
    {
        public string Id { get; set; }
    }
}
