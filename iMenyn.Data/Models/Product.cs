using System.Collections.Generic;

namespace iMenyn.Data.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<ProductPrice> Prices { get; set; }
        public string Image { get; set; }


        //List of ids who liked
        public List<string> Likes { get; set; }

        // Dryck
        public float Abv { get; set; }
        public int Size { get; set; }
    }

    public class ProductPrice
    {
        public int Price { get; set; }
        public string Description { get; set; }
    }
}