using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;

namespace iMenyn.Web.Controllers
{
    public class CabaretController : BaseController
    {
        public CabaretController(IDb db, ILogger logger)
            : base(db, logger) { }

        public ActionResult Index(string c)
        {
            if (!string.IsNullOrEmpty(c))
            {
                //TODO
                //var viewModel = Repository.GetMenu(c);
                //return View(viewModel);
            }
            return View();
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
