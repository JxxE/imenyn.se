using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rantup.Data.Infrastructure;
using Rantup.Data.Models;
using Rantup.Web.Extensionmethods;

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

        public static string CreateId(DocumentType doc)
        {
            string document;
            switch (doc)
            {
                case DocumentType.StartPage:
                    document = "startPages";
                    break;
                case DocumentType.Category:
                    document = "categories";
                    break;
                case DocumentType.SubCategory:
                    document = "subCategories";
                    break;
                case DocumentType.Product:
                    document = "product";
                    break;
                default:
                    document = "IdGenerationFail";
                    break;
            }            
            return string.Format("{0}-{1}{2}",document,TimeToUnix(DateTime.Now), Utility.GetKeyByAccountId(GetCurrentAccount().Id)); 
        }

        private static string TimeToUnix(DateTime time)
        {
            return (time - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds.ToString().Replace(",","");
        }
    }

    public enum DocumentType
    {
        StartPage,
        Category,
        SubCategory,
        Product
    }

}