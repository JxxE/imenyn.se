using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rantup.Data.Models;

namespace Rantup.Data.Helpers
{
    public static class MenuHelper
    {
        public static string GetId(string enterpriseKey)
        {
            return string.Format("menu-{0}", enterpriseKey);
        }
    }
}