using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Common.Validation
{
    public static class Methods
    {
        public static bool NotIncludeSpecialCharacters(string name)
        {
            var regex = new Regex("^[a-zA-Z]*$");

            if (regex.IsMatch(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
