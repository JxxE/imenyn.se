using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iMenyn.Data.Attributes
{
    public class AdminOnlyAttribute:ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            
        }
    }
}