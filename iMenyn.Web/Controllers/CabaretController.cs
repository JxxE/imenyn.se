using System.Web.Mvc;
using iMenyn.Data.Abstract;

namespace iMenyn.Web.Controllers
{
    public class CabaretController : BaseController
    {
        public CabaretController(IRepository repository) : base(repository) { }

        public ActionResult Index(string c)
        {
            if (!string.IsNullOrEmpty(c))
            {
                var viewModel = Repository.GetMenu(c);
                return View(viewModel);
            }
            return View();
        }

        public ActionResult Product(string id)
        {
            var product = Repository.GetProductById(id);
            ViewBag.Product = product;
            return View();
        }


    }
}
