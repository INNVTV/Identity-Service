using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Core.Common.Validation
{
    public class ReservedNames
    {
        public static readonly ReadOnlyCollection<string> ReservedUserNames = new ReadOnlyCollection<string>(new[]
        {
            #region Reserved UserNames
            
            //a
            "admin",
            "account",
            "accounts",
            "accountname",
            "accountnamekey",
            "active",
            "api",

            //b

            //c

            //d
            "dateCreated",
            "documenttype",
            "default",

            //e
            
            //f
            "filepath",
            "ftp",
            "fullyqualifiedname",

            //g

            //h

            //i
            "id",
            "image",
            "images",

            //j

            //k

            //l
            "locationpath",
            "locations",

            //m
            "metadata",
            "locationmetadata",

            //n
            "name",
            "namekey",
            "null",

            //o

            //p
            "properties",
            "property",
            
            //q

            //r

            //s
            "selflink",
            "search",

            "sort",
            "sorting",
            "sort-by",
            "sortby",

            //t
            "tags",
            "tag",
            "thumbnails",
            "thumbnail",
            "title",
            //u

            //v
            "visible"

            //w

            //x

            //y

            //z

            #endregion

        });
    }
}
