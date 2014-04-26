using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Concrete.Db
{
    public class RavenDbMenus : IDbMenus
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger _logger;

        public RavenDbMenus(IDocumentStore documentStore, ILogger logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public Menu GetMenuById(string menuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Menu>(menuId);
            }
        }

        //Former GetMenu
        public CompleteEnterpriseViewModel GetMenuByEnterpriseKey(string enterpriseKey)
        {
            using (var session = _documentStore.OpenSession())
            {
                //var enterprise = session.Include<Enterprise>(x => x.Products).Load(EnterpriseHelper.GetId(enterpriseKey));


                //var products = new List<Product>();
                //foreach (var p in enterprise.Products)
                //{
                //    var product = session.Load<Product>(p);
                //    products.Add(product);
                //}

                //var viewModel = new CompleteEnterpriseViewModel
                //{

                //};

                return null;
            }
        }

        public ModifiedMenu GetModifiedMenuByEnterpriseId(string enterpriseId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<ModifiedMenu>().FirstOrDefault(m => m.EnterpriseId == enterpriseId);
            }
        }

        public ModifiedMenu GetModifiedMenuById(string modifiedMenuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<ModifiedMenu>(modifiedMenuId);
            }
        }


        public void UpdateMenu(Menu menu)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(menu);
                session.SaveChanges();
            }
        }

        public void DeleteMenuById(string menuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var menu = session.Load<Menu>(menuId);
                session.Delete(menu);
                session.SaveChanges();
            }
        }

        [Obsolete]
        public void DeleteModifiedMenuById(string modifiedMenuId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var modifiedMenu = session.Load<ModifiedMenu>(modifiedMenuId);
                session.Delete(modifiedMenu);
                session.SaveChanges();
            }
        }

        public void CreateMenu(Menu menu, List<Product>products)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(menu);
                

                foreach (var product in products)
                {
                    session.Store(product);    
                }
                
                session.SaveChanges();
            }
        }

        [Obsolete]
        public void CreateModifiedMenu(ModifiedMenu modifiedMenu)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(modifiedMenu);
                session.SaveChanges();
            }
        }
    }
}