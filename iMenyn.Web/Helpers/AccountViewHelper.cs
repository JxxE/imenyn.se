using System.Web;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;
using iMenyn.Web.ViewModels;
using AutoMapper;

namespace iMenyn.Web.Helpers
{
    public class AccountViewHelper
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
                currentAccount = DependencyManager.Db.Accounts.GetAccount(HttpContext.Current.User.Identity.Name);
                HttpContext.Current.Items.Add(cacheKey, currentAccount);
            }

            return currentAccount;
        }

        public static AccountViewModel ModelToViewModel(Account account)
        {
            var accountViewModel = new AccountViewModel();
            Mapper.CreateMap<Account, AccountViewModel>();
            Mapper.Map(account, accountViewModel);
            return accountViewModel;
        }
    }
}