using Core.Common.Response;
using Core.Domain.Entities;
using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Application.Authentication.Models
{
    public class AuthenticationResponse : BaseResponse
    {
        public AuthenticationResponse()
            : base()
        {

        }

        public AuthenticationResponse(IList<ValidationFailure> failures)
            : base(failures)
        {

        }

        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
    }
}