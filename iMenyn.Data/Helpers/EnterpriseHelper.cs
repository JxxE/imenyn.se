using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static List<string> GenerateSearchTags(string enterpriseName)
        {
            var tags = new List<string>();
            var cleanEnterpriseName = RemoveSpecialCharacters(enterpriseName).ToLower();

            var enterpriseNames = cleanEnterpriseName.Split(' ');
            tags.AddRange(enterpriseNames);

            return FilterUnwantedTags(tags);
        }

        private static string RemoveSpecialCharacters(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Ö') || (c >= 'a' && c <= 'ö'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static List<ValueAndText> GetDisplayCategories(List<string> categoryIds)
        {
            return (from categoryId in categoryIds
                    from systemCategory in GeneralHelper.GetCategories()
                    where categoryId == systemCategory.Value
                    select systemCategory).ToList();
        }

        public static List<ValueAndText> GetDisplayCategories(List<ValueAndText> list)
        {
            return GetDisplayCategories(list.Select(p => p.Value).ToList());
        }


        private static List<string> FilterUnwantedTags(IEnumerable<string> tags)
        {
            var blackList = new List<string>
                                {
                                    "restaurang","grill","gatukök","pizzeria"
                                };

            return tags.Where(tag => !blackList.Contains(tag.ToLower())).ToList();
        }
    }
}