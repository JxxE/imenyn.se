using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Abstract
{
    public interface IRepository
    {
        bool AddAccount(Account account);
        IEnumerable<Account> GetAccounts();
        Account GetUserByEmail(string userName);
        Account GetAccount(string accountId);
        void UpdateAccount(Account account);

        void AddProduct(Product product);



        IEnumerable<Product> GetProducts(List<string> productIds);
        Product GetProductById(string productId);

        object GetPage(string id);

        Enterprise GetEnterpriseById(string enterpriseId);
        IEnumerable<Enterprise> GetAllEnterprises();
        IEnumerable<Enterprise> GetNewEnterprises();
        IEnumerable<Enterprise> GetEnterprisesWithModifiedMenus();
        Enterprise GetEnterpriseByUrlKey(string urlKey);
        IEnumerable<Enterprise> GetEnterprisesByLocation(string stateCode, string city);

        IEnumerable<ModifiedMenu> GetAllModifiedMenus();
        ModifiedMenu GetModifiedMenuByEnterpriseId(string enterpriseId);
        ModifiedMenu GetModifiedMenuById(string modifiedMenuId);
       
        void UpdateEnterprise(Enterprise enterprise);

        Menu GetMenuById(string menuId);
        MenuViewModel GetMenu(string enterpriseKey);
        //Update
        void UpdateProduct(Product product);

        //New
        IEnumerable<Enterprise> SearchEnterprises(string searchTerm, string location, string categorySearch);
        IEnumerable<Enterprise> CheckIfEnterpriseExists(string key, int postalCode);

        void UpdateMenu(Menu menu);

        void UpdateProducts(IEnumerable<Product> products);

        void DeleteEnterpriseById(string enterpriseId);
        void DeleteMenuById(string menuId);
        void DeleteProductsByIds(List<string> productIds);
        void DeleteModifiedMenuById(string modifiedMenuId);

        void CreateEnterprise(Enterprise enterprise);
        void CreateMenu(Menu menu);
        void CreateProducts(IEnumerable<Product> products);
        void CreateModifiedMenu(ModifiedMenu modifiedMenu);
    }
}
