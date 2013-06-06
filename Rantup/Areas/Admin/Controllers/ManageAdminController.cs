using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Data.Concrete;
using Rantup.Data.Helpers;
using Rantup.Data.Models;
using Rantup.Web.Areas.Admin.ViewModels;
using Rantup.Web.Controllers;
using Rantup.Web.Helpers;
using Rantup.Web.Infrastructure;
using Rantup.Web.ViewModels;

namespace Rantup.Web.Areas.Admin.Controllers
{
    [OnlyAdmin]
    public class ManageAdminController : BaseController
    {
        //
        // GET: /Admin/Admin/

        public ManageAdminController(IRepository repository, IAuthentication authentication = null)
            : base(repository, authentication)
        {
        }

        public ActionResult Index()
        {
            return View();
        }



        //Show all new contributions
        public ActionResult NewEnterprises()
        {
            var model = new AllEnterprisesViewModel
                                {
                                    Enterprises = Repository.GetAllEnterprises().Where(e => e.IsTemp)
                                };
            return View(model);
        }

        //Display the new temp-menu
        public ActionResult NewMenu(string enterpriseId)
        {
            var enterprise = Repository.GetEnterpriseById(enterpriseId);
            var menu = Repository.GetMenuById(enterprise.Menu);
            var products = Repository.GetProducts(menu.Products.ToList());

            var model = ViewModelHelper.CreateStandardViewModel(enterprise, products);

            return View(model);
        }

        #region Modified menus
        //Show all enterprises with modified menus
        public ActionResult ModifiedMenus()
        {
            var modifiedMenus = Repository.GetAllModifiedMenus();
            var enterprises = modifiedMenus.Select(modifiedMenu => Repository.GetEnterpriseById(modifiedMenu.EnterpriseId)).ToList();

            var model = new AllEnterprisesViewModel
            {
                Enterprises = enterprises
            };
            return View(model);
        }
        public ActionResult ModifiedMenu(string enterpriseKey)
        {
            var enterpriseId = EnterpriseHelper.GetId(enterpriseKey);

            var modifiedMenu = Repository.GetModifiedMenuByEnterpriseId(enterpriseId);

            var enterprise = Repository.GetEnterpriseById(enterpriseId);

            if (enterprise != null)
            {
                var liveMenu = Repository.GetMenuById(enterprise.Menu);
                if (liveMenu != null)
                {
                    var modifiedProductsViewModel = new ProductListViewModel();
                    var modifiedMenuId = "";

                    if (modifiedMenu != null)
                    {
                        var modifiedMenuProducts = Repository.GetProducts(modifiedMenu.ProductIds).ToList();
                        modifiedProductsViewModel = ViewModelHelper.GetProductListViewModel(modifiedMenuProducts);
                        modifiedMenuId = modifiedMenu.Id;
                    }

                    var liveMenuProducts = Repository.GetProducts(liveMenu.Products.ToList()).ToList();
                    var liveMenuProductsViewModel = ViewModelHelper.GetProductListViewModel(liveMenuProducts);
                    

                    var viewModel = new CompareModifiedMenuViewModel
                                        {
                                            Enterprise = enterprise,
                                            ModifiedMenuProducts = modifiedProductsViewModel,
                                            LiveMenuProducts = liveMenuProductsViewModel,
                                            ModifiedMenuId = modifiedMenuId
                                        };
                    return View(viewModel);
                }
            }
            return View();
        }
        public RedirectToRouteResult ApproveModifiedMenu(string enterpriseKey)
        {
            var modifiedMenuId = ModifiedMenuHelper.GetId(enterpriseKey);
            var modifiedMenu = Repository.GetModifiedMenuById(modifiedMenuId);

            if (modifiedMenu == null) return RedirectToAction("Index");

            var liveMenuId = MenuHelper.GetId(enterpriseKey);
            var liveMenu = Repository.GetMenuById(liveMenuId);

            if (liveMenu == null) return RedirectToAction("Index");

            var liveMenuProductList = liveMenu.Products.ToList();
            var oldProductIds = new List<string>();

            foreach (var productId in liveMenuProductList)
            {
                oldProductIds.Add(productId);
            }

            liveMenuProductList.Clear();
            liveMenuProductList.AddRange(modifiedMenu.ProductIds);

            liveMenu.Products = liveMenuProductList;

            Repository.UpdateMenu(liveMenu);
            Repository.DeleteModifiedMenuById(modifiedMenuId);
            Repository.DeleteProductsByIds(oldProductIds);

            return RedirectToAction("ModifiedMenu", "ManageAdmin", new { enterpriseKey });

        }
        public RedirectToRouteResult DisapproveModifiedMenu(string modifiedMenuId)
        {
            Repository.DeleteModifiedMenuById(modifiedMenuId);
            return RedirectToAction("ModifiedMenus");
        }
        #endregion

        //Convert the temp to a real one.
        public RedirectToRouteResult ApproveMenu(string enterpriseId)
        {
            var enterprise = Repository.GetEnterpriseById(enterpriseId);

            enterprise.IsTemp = false;

            Repository.UpdateEnterprise(enterprise);

            return RedirectToAction("NewEnterprises");
        }

        //Delete the temporary menu.
        public RedirectToRouteResult DisapproveMenu(string enterpriseId)
        {
            var enterprise = Repository.GetEnterpriseById(enterpriseId);
            var menu = Repository.GetMenuById(enterprise.Menu);

            Repository.DeleteEnterpriseById(enterprise.Id);
            Repository.DeleteMenuById(menu.Id);
            Repository.DeleteProductsByIds(menu.Products.ToList());

            return RedirectToAction("NewEnterprises");
        }


        [HttpPost]
        public ActionResult CreateAllIndexes()
        {
            RavenContext.Instance.CreateAllIndexes();
            ViewBag.Message = "Successfully created all RavenDB indexes";
            return View("Index");
        }
    }
}
