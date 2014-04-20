using System.Web;
using iMenyn.Data.Infrastructure;

namespace iMenyn.Web.Extensionmethods
{
    public class Utility
    {
        public static string GetTypeById(string id)
        {
            var type = "";
            if (id.Contains("categories"))
                type = "category";
            if (id.Contains("subCategories"))
                type = "sub-category";
            if (id.Contains("product"))
                type = "product";
            return type;
        }

        public static string GetKeyByAccountId(string accountId)
        {
            //TODO
            //var repository = DependencyManager.Repository;
            //var account = repository.GetAccount(accountId);
            //var enterprise = repository.GetEnterpriseById(account.Enterprise);
            //return enterprise.Key;
            return null;
        }

        public static void UploadImage(HttpPostedFileBase file, string accountId)
        {
            var path = HttpContext.Current.Server.MapPath("~/r/" + GetKeyByAccountId(accountId) + "/");
            file.SaveAs(path + file.FileName);
        }

        public static string GetImageUrl(string key, string filename)
        {
            return string.Format("{0}{1}", GetImageDirectoryPath(key), filename);
        }

        public static string GetImageDirectoryPath(string key)
        {
            return string.Format("/r/{0}/", key);
        }
    }
}