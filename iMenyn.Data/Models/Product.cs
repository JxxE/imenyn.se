using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iMenyn.Data.Models
{
    public class Product
    {
        public string Id { get; set; }
        [Display(Name = "Namn")]
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
    }

    public class ProductPrice
    {
        public int Price { get; set; }
        public string Description { get; set; }
    }
}