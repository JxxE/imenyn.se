using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iMenyn.Data.Models
{
    public class Product : BasicProduct
    {
        public string Id { get; set; }
        [Display(Name = "Produkt")]
        public string Name { get; set; }
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Pris")]
        public List<ProductPrice> Prices { get; set; }
        [Display(Name = "Bild")]
        public string Image { get; set; }


        //List of ids who liked
        public List<string> Likes { get; set; }

        // Dryck
        [Display(Name ="Alkoholhalt")]
        public float Abv { get; set; }

        public string Enterprise { get; set; }

        public ProductUpdatedVersion UpdatedVersion { get; set; }
    }

    public interface BasicProduct
    {
        string Name { get; set; }
        string Description { get; set; }
        List<ProductPrice> Prices { get; set; }
        string Image { get; set; }
        float Abv { get; set; }
    }

    public class ProductPrice
    {
        public int Price { get; set; }
        public string Description { get; set; }
    }


    public class ProductUpdatedVersion:BasicProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ProductPrice> Prices { get; set; }
        public string Image { get; set; }
        public float Abv { get; set; }

    }
}