using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Core.Common.Response;
using IdentityService.ServiceModels;
using AutoMapper;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Models.Views;
using Core.Application.Users.Models.Enums;
using Core.Application.Users.Queries.GetUsersList;
using Core.Application.Users.Queries.GetUserByUserName;
using Core.Application.Users.Queries.GetUserByEmail;
using Core.Application.Users.Queries.SearchUsers;
using Core.Application.Users.Commands.DeleteUser;
using Core.Application.Users.Commands.UpdateUserName;
using Core.Application.Users.Commands.UpdateEmail;
using Core.Application.Users.Commands.AddToRole;
using Core.Application.Users.Commands.RemoveFromRole;
using Core.Application.Users.Queries.GetUserById;
using Core.Application.Passwords.Commands.UpdatePassword;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Controllers
{
    
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper; //<-- Instance version of IMapper. Used only in the Service layer for ServiceModels

        public UsersController(IServiceProvider serviceProvider, IMapper mapper)
        {
            _mediator = serviceProvider.GetService<IMediator>();
            _mapper = mapper;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<CreateUserCommandResponse> Post(CreateUserServiceModel createUserServiceModel)
        {
            //Use AutoMapper instance to transform ServiceModel into MediatR Request (Configured in Startup)
            var createUserCommand = _mapper.Map<CreateUserCommand>(createUserServiceModel);

            var result = await _mediator.Send(createUserCommand);
            return result;
        }

        [Route("list")]
        [HttpGet]
        public async Task<UsersListResultsViewModel> List(int pageSize = 20, OrderBy orderBy = OrderBy.UserName, OrderDirection orderDirection = OrderDirection.ASC, string continuationToken = null)
        {
            // We don't use the GetUserListQuery in the controller method otherwise Swagger tries to use a POST on our GET call
            var userListQuery = new GetUsersListQuery { PageSize = pageSize, OrderBy = orderBy, OrderDirection = orderDirection, ContinuationToken = continuationToken };
            var result = await _mediator.Send(userListQuery);
            return result;

            //-----------------------------------------------------
            // TODO: DocumentDB will soon have skip/take
            // For now we use continuation token to get next batch from list
            // For even more robust query capabilities you should use the 'search' route
            //-----------------------------------------------------
        }

        [Route("search")]
        [HttpGet]
        public async Task<UserSearchResultsViewModel> Search(string queryString, int page, int pageSize = 20, OrderBy orderBy = OrderBy.UserName, OrderDirection orderDirection = OrderDirection.ASC)
        {
            // We don't use the GetAccountListQuery in the controller method otherwise Swagger tries to use a POST on our GET call
            var accountListQuery = new SearchUsersQuery { QueryString = queryString, Page = page, PageSize = pageSize, OrderBy = orderBy, OrderDirection = orderDirection };
            var result = await _mediator.Send(accountListQuery);
            return result;

            //-----------------------------------------------------
            // Uses Azure Search
            //-----------------------------------------------------
        }


        [Route("username/{username}")]
        [HttpGet]
        public async Task<UserDetailsViewModel> ByUserName(string username)
        {
            var getUserByUserNameQuery = new GetUserByUserNameQuery() { UserName = username };
            return await _mediator.Send(getUserByUserNameQuery);
        }

        [Route("email/{email}")]
        [HttpGet]
        public async Task<UserDetailsViewModel> Byemail(string email)
        {
            var getUserByEmailQuery = new GetUserByEmailQuery() { Email = email };
            return await _mediator.Send(getUserByEmailQuery);
        }

        [Route("id/{id}")]
        [HttpGet]
        public async Task<UserDetailsViewModel> ById(string id)
        {
            var getUserByIdQuery = new GetUserByIdQuery() { Id = id };
            return await _mediator.Send(getUserByIdQuery);
        }

        [Route("update/username")]
        [HttpPut]
        public async Task<BaseResponse> UpdateUserName(UpdateUserNameCommand updateUserNameCommand)
        {
            var result = await _mediator.Send(updateUserNameCommand);
            return result;
        }

        [Route("update/email")]
        [HttpPut]
        public async Task<BaseResponse> UpdateEmail(UpdateEmailCommand updateEmailCommand)
        {
            var result = await _mediator.Send(updateEmailCommand);
            return result;
        }

        [Route("update/password")]
        [HttpPut]
        public async Task<BaseResponse> UpdatePassword(UpdatePasswordCommand updatePasswordCommand)
        {
            var result = await _mediator.Send(updatePasswordCommand);
            return result;
        }

        [Route("roles/add")]
        [HttpPut]
        public async Task<BaseResponse> AddUserToRole(AddToRoleCommand addUserToRoleCommand)
        {
            var result = await _mediator.Send(addUserToRoleCommand);
            return result;
        }

        [Route("roles/remove")]
        [HttpDelete]
        public async Task<BaseResponse> RemoveUserFromRole(RemoveFromRoleCommand removeUserFromRoleCommand)
        {
            var result = await _mediator.Send(removeUserFromRoleCommand);
            return result;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<BaseResponse> Delete(string id)
        {
            var deleteCommand = new DeleteUserCommand() { Id = id };
            return await _mediator.Send(deleteCommand);
        }
        
    }
}
