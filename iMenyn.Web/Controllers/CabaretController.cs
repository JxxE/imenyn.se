using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Helpers;

namespace iMenyn.Web.Controllers
{
    public class CabaretController : BaseController
    {
        public CabaretController(IDb db, ILogger logger)
            : base(db, logger) { }

        public ActionResult Index(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                var enterprise = Db.Enterprises.GetCompleteEnterprise(EnterpriseHelper.GetId(q));
                return View(enterprise);
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult Product(string id)
        {
            //TODO
            //var product = Repository.GetProductById(id);
            //ViewBag.Product = product;
            return View();
        }


    }
}
