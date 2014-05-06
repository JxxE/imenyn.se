using System.Web;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;
using AutoMapper;

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

        public static EnterpriseViewModel ModelToViewModel(Enterprise model)
        {
            Mapper.CreateMap<Enterprise, EnterpriseViewModel>();
            return Mapper.Map<Enterprise, EnterpriseViewModel>(model);
        }
    }
}