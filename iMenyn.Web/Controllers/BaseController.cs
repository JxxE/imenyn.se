using System;
using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Infrastructure;

namespace iMenyn.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IDb Db;
        protected ILogger Logger;
        protected IAuthentication Authentication;

        public BaseController(IDb db, ILogger logger, IAuthentication authentication = null)
        {
            Db = db;
            Logger = logger;

            Authentication = authentication ?? DependencyManager.Authentication;
        }

    }
}
