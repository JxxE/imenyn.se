using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iMenyn.Data.Helpers
{
    public class ModifiedMenuHelper
    {
        public static string GetId(string enterpriseKey)
        {
            return string.Format("modifiedMenu-{0}", enterpriseKey);
        }
    }
}