using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Web.Controllers
{
    public class ManageController : BaseController
    {
        public ManageController(IDb db, ILogger logger)
            : base(db, logger)
        {
        }

        public ActionResult Index(string key)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult DemoMenu(bool edit=false)
        {
            var viewModel = Db.Enterprises.GetCompleteEnterprise("enterprises-jessetinell",edit);
            return View(viewModel);
        }


        // Detta gäller en NY enterprise. När man redigerar en nuvarande enterprise måste det sparas en TEMP-meny!
        [HttpPost]
        public int AddOrEditNewProduct(ProductViewModel product)
        {
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.Name) && (product.Prices != null && product.Prices.Count > 0))
                {
                    var p = ProductHelper.ViewModelToModel(product);
                    if (product.Id != null && Db.Products.GetProductById(product.Id) != null)
                    {
                        Db.Products.UpdateProduct(p,product.EnterpriseId);
                        return 10;
                    }

                    Db.Products.AddProduct(p, product.CategoryId, product.EnterpriseId);
                    return 20;
                }
            }
            return 30;
        }

        //Sparar ordningen på menyn. Kategori, produkt-placering 
        [HttpPost]
        public void SaveMenuSetup(Menu menu, string enterpriseId)
        {
            Db.Enterprises.UpdateEnterprise(enterpriseId, menu);
        }

        public ViewResult BlankCategory(string enterpriseId)
        {
            return View("_Category", new ViewModelCategory{Id=GeneralHelper.GetGuid(),Name = string.Empty,Products = new List<ProductViewModel>{new ProductViewModel{Id = ProductHelper.GenerateId()}},EnterpriseId = enterpriseId});
        }
        public ViewResult BlankProduct(string enterpriseId, string categoryId)
        {
            return View("_Product", new ProductViewModel { Id = ProductHelper.GenerateId(), EnterpriseId = enterpriseId, CategoryId = categoryId });
        }
        public ViewResult BlankProductPrice()
        {
            return View("_ProductPrice", new ProductPrice());
        }
    }
}
