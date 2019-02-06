using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Constants
{
    public static class CachingKeys
    {
        // Login Lockout Attempts  --------------

        public static string LoginAttempts(string userNameOrEmail)
        {
            return $"login:attempts:{userNameOrEmail.ToLower().Trim()}";
        }

        // Password Reset Code  --------------

        public static string PasswordResetCode(string resetCode)
        {
            return $"password:reset:{resetCode}";
        }
    }
}
