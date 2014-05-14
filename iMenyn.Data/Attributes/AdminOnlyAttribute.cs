using System;
using System.Web;
using System.Web.Mvc;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;

namespace iMenyn.Data.Attributes
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (var session = DependencyManager.DocumentStore.OpenSession())
            {
                try
                {
                    return session.Load<Account>(String.Format("{0}", HttpContext.Current.User.Identity.Name)).IsAdmin;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
        }
    }
}