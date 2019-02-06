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
            User = null;
        }

        public ResetRequestModel(string resetCode, User user)
        {
            IsValid = true;
            ResetCode = resetCode;
            User = user;
        }

        public bool IsValid { get; set; }
        public string ResetCode { get; set; }
        public User User { get; set; }
    }
}
