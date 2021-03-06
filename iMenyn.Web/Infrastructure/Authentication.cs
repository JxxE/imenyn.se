﻿using System.Web.Security;
using iMenyn.Data.Abstract;

namespace iMenyn.Web.Infrastructure
{
    public class Authentication : IAuthentication
    {
        #region Implementation of IAuthentication

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public void SetAuthCookie(string username, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(username, createPersistentCookie);
        }

        public void SetAuthCookie(string username, bool createPersistentCookie, string cookiePath)
        {
            FormsAuthentication.SetAuthCookie(username, createPersistentCookie);
        }

        #endregion
    }
}