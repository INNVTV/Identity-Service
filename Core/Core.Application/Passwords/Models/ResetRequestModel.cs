using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Passwords.Models
{
    public class ResetRequestModel
    {
        public ResetRequestModel()
        {
            IsValid = false;
            ResetCode = null;
            UserId = null;
        }

        public ResetRequestModel(string resetCode, string userId)
        {
            IsValid = true;
            ResetCode = resetCode;
            UserId = userId;
        }

        public bool IsValid { get; set; }
        public string ResetCode { get; set; }
        public string UserId { get; set; }
    }
}
