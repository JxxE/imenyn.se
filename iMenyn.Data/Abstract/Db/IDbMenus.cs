using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbMenus
    {
        Menu GetMenuById(string menuId);
        MenuViewModel GetMenuByEnterpriseKey(string enterpriseKey);
        ModifiedMenu GetModifiedMenuByEnterpriseId(string enterpriseId);
        ModifiedMenu GetModifiedMenuById(string modifiedMenuId);
        IEnumerable<ModifiedMenu> GetAllModifiedMenus();
        void UpdateMenu(Menu menu);
        void DeleteMenuById(string menuId);
        void DeleteModifiedMenuById(string modifiedMenuId);
        void CreateMenu(Menu menu);
        void CreateModifiedMenu(ModifiedMenu modifiedMenu);
    }
}
