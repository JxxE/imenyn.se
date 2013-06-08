using System.Web;
using Rantup.Data.Infrastructure;
using Rantup.Data.Models;

namespace Rantup.Web.Helpers
{
    public class AccountHelper
    {
        public static Account GetCurrentAccount()
        {
            Account currentAccount = null;
            const string cacheKey = "currentUser";

            if (HttpContext.Current.Items.Contains(cacheKey))
                currentAccount = HttpContext.Current.Items[cacheKey] as Account;

            if (currentAccount == null)
            {
                // Load account by ID
                currentAccount = DependencyManager.Repository.GetAccount(HttpContext.Current.User.Identity.Name);
                HttpContext.Current.Items.Add(cacheKey, currentAccount);
            }

            return currentAccount;
        }
    }
}