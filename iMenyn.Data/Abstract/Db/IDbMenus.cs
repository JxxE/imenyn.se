using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbMenus
    {
        Menu GetMenuById(string menuId);
        CompleteEnterpriseViewModel GetMenuByEnterpriseKey(string enterpriseKey);
        ModifiedMenu GetModifiedMenuByEnterpriseId(string enterpriseId);
        ModifiedMenu GetModifiedMenuById(string modifiedMenuId);
        void UpdateMenu(Menu menu);
        void DeleteMenuById(string menuId);
        void DeleteModifiedMenuById(string modifiedMenuId);
        void CreateMenu(Menu menu, List<Product> products);
        void CreateModifiedMenu(ModifiedMenu modifiedMenu);
    }
}
