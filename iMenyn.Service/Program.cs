using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iMenyn.Data;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Concrete;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;

namespace iMenyn.Service
{
    class Program
    {
        private static IDb _db;
        private static ILogger _logger;

        static void Main(string[] args)
        {
            // Hook up
            DependencyManager.InjectRavenDbContext(RavenContext.Instance);
            _db = DependencyManager.Db;
            _logger = DependencyManager.Logger;

            var action = Action.NotSet;

            Console.WriteLine("Choose action:");
            Console.WriteLine("1. Add ids to products");

            Console.WriteLine("");
            Console.WriteLine("--- Update scripts ---");

            var input = Console.ReadLine();

            action = (Action)Convert.ToInt32(input);

            if(action == Action.AddIdToProducts)
            {
             var products = _db.Products.GetAllProductsInDb();
                var en = products as Product[] ?? products.ToArray();
                foreach (var product in en)
                {
                    product.Enterprise = "enterprises-jessetinell";
                }
                _db.Products.UpdateProducts(en);
            }
        }

        internal enum Action
        {
            NotSet = 0,
            AddIdToProducts = 1
        }
    }
}
