using Core.Application.Users.Queries.GetUserByEmail;
using Core.Application.Users.Queries.GetUserByUserName;
using Core.Common.Response;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.DocumentDatabase;
using Core.Infrastructure.Persistence.RedisCache;
using Core.Infrastructure.Services.Email;
using FluentValidation.Results;
using MediatR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Passwords.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, BaseResponse>
    {
        //MediatR will automatically inject dependencies
        private readonly IMediator _mediator;
        private readonly ICoreConfiguration _coreConfiguration;
        private readonly IRedisContext _redisContext;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(IMediator mediator, ICoreConfiguration coreConfiguration, IRedisContext redisContext, IEmailService emailService)
        {
            _mediator = mediator;
            _coreConfiguration = coreConfiguration;
            _redisContext = redisContext;
            _emailService = emailService;
        }

        public async Task<BaseResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            ForgotPasswordValidator validator = new ForgotPasswordValidator();
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new BaseResponse(validationResult.Errors) { Message = "One or more validation errors occurred" };
            }

            // Get user (if exists)
            var user = await _mediator.Send(new GetUserByUserNameQuery { UserName = request.UserNameOrEmail });
            if(user == null)
            {
                user = await _mediator.Send(new GetUserByEmailQuery { Email = request.UserNameOrEmail });
            }

            if(user != null)
            {
                #region Setup our caching client and key

                IDatabase cache = _redisContext.ConnectionMultiplexer.GetDatabase();
                var passwordResetCode = Guid.NewGuid().ToString();
                var resetCodeCacheKey = Common.Constants.CachingKeys.PasswordResetCode(passwordResetCode);

                #endregion

                var cached = cache.StringSet(resetCodeCacheKey, user.Id.ToString(), TimeSpan.FromHours(_coreConfiguration.Logins.PasswordResetTimespanHours), When.Always, CommandFlags.None);

                if(cached)
                {
                    //=========================================================================
                    // SEND EMAIL 
                    //=========================================================================

                    var resetLink = String.Concat(_coreConfiguration.Endpoints.Domain, "/password/reset/", passwordResetCode);

                    var emailMessage = new EmailMessage
                    {
                        ToEmail = user.Email,
                        ToName = String.Concat(user.FirstName, " ", user.LastName),
                        Subject = "Password Reset",
                        TextContent = String.Concat("Hello ", String.Concat(user.FirstName, " ", user.LastName), ". A password reset request has been made on your account. If you made this request please reset your password by following the instructions at this link: ", resetLink),
                        HtmlContent = String.Concat("Hello ", String.Concat(user.FirstName, " ", user.LastName), ",<br/><br/> A password reset request has been made on your account. If you made this request please reset your password by following the instructions <a href='", resetLink, "'>here</a>"),
                    };

                    try
                    {
                        var emailSent = await _emailService.SendEmail(emailMessage);
                    }
                    catch (Exception ex)
                    {
                        // throw EmailServiceException (if a custom exception type is desired)
                        // ... Will be caught, logged and handled by the ExceptionHandlerMiddleware

                        // Pass the exception up the chain:
                        throw ex;

                        // ...Or pass along as inner exception:
                        //throw new Exception("An error occured trying to use the email service", ex);
                    }
                    finally
                    {
                        // Use alternate communication method...
                    }
                }

            }

            // NOTE: We return the same response regardless of finding a user or not.
            // This prevents anyone from checking to see if a username or email is in the system
            return new BaseResponse { isSuccess = true, Message = "Thank you. If this is a valid account reset instructions have been sent to the email address on file" };
        }
    }
}
