using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rantup.Data.Models;

namespace Rantup.Data.Abstract
{
    public interface IRepository
    {
        void AddAccount(Account account);
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
        Enterprise GetEnterpriseByUrlKey(string urlKey);

        IEnumerable<ModifiedMenu> GetAllModifiedMenus();
        ModifiedMenu GetModifiedMenuByEnterpriseId(string enterpriseId);
        ModifiedMenu GetModifiedMenuById(string modifiedMenuId);
       
        void UpdateEnterprise(Enterprise enterprise);

        Menu GetMenuById(string menuId);

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
