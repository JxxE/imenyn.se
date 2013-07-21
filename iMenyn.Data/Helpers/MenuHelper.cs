using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iMenyn.Data.Models;

namespace iMenyn.Data.Helpers
{
    public static class MenuHelper
    {
        public static string GetId(string enterpriseKey)
        {
            return string.Format("menu-{0}", enterpriseKey);
        }
    }
}