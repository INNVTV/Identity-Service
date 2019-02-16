using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Common.Validation
{
    public static class Methods
    {
        public static bool NotIncludeSpacesOrSpecialCharacters(string str)
        {
            var regex = new Regex("^[a-zA-Z0-9]*$");

            if (regex.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool NotIncludeNumbersSpacesOrSpecialCharacters(string str)
        {
            var regex = new Regex("^[a-zA-Z]*$");

            if (regex.IsMatch(str))
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
