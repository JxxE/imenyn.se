using System.Web;
using iMenyn.Data.Models;

namespace iMenyn.Data.Helpers
{
    public class EnterpriseHelper
    {
        public static string GetId(string key)
        {
            return string.Format("enterprises-{0}", key);
        }

        public static string GetKey(string id)
        {
            return id.Replace("enterprises-", string.Empty);
        }

        public static bool ValidEditableEnterprise(Enterprise enterprise, Raven.Client.IDocumentSession session)
        {
            if(enterprise != null)
            {
                if (enterprise.OwnedByAccount)
                {
                    //If enterprise is owned by an account, check if current account is the correct one
                    var account = session.Load<Account>(HttpContext.Current.User.Identity.Name);
                    return (account.Enterprises.Contains(enterprise.Id) || account.IsAdmin) && account.Enabled;
                }
                //Add product to a new enterprise
                if (enterprise.IsNew)
                    return true;
                //Add product to an enterprise in edit-mode
                if (!enterprise.LockedFromEdit)
                    return true;
            }

            return false;
        }

        //private static string PossibleEnterPriseKey(string enterpriseName,int amout)
        //{
        //    var sb = new StringBuilder();
        //    foreach (var c in enterpriseName.Where(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == 'å' || c == 'Å' || c == 'ä' || c == 'Ä' || c == 'ö' || c == 'Ö'))
        //    {
        //        switch (c)
        //        {
        //            case 'å':
        //            case 'ä':
        //            case 'Å':
        //            case 'Ä':
        //                sb.Append('a');
        //                break;
        //            case 'ö':
        //            case 'Ö':
        //                sb.Append('o');
        //                break;
        //            default:
        //                sb.Append(c);
        //                break;
        //        }
        //    }
        //    var possibleKey = sb.ToString().ToLower();

        //    //Increment possible key if it one with same key exists. Ex. jensens1
        //    if (amout > 0)
        //        possibleKey += amout;
        //    if (amout > 28)
        //    {
        //        //Extreme case! If there are over 28 documents with the keys tried, use a random string instead. Ravendb has a limit of 30 requests per client session
        //        possibleKey += GeneralHelper.RandomString(8);
        //    }

        //    return possibleKey;
        //}
    }
}