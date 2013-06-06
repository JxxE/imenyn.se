using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rantup.Data.Helpers
{
    public class EnterpriseHelper
    {
        public static string GetId(string key)
        {
            return string.Format("enterprises-{0}",key);
        }
    }
}