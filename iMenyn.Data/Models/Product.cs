using System;
using System.Collections.Generic;

namespace iMenyn.Data.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public float Abv { get; set; }
        public int Size { get; set; }
        public string Image { get; set; }

        public string Category { get; set; }
        public string ProductType { get; set; }

        //Good for i.e. wine. 1 glas 59kr o kommentaren (Flaska 500kr).
        public string AdditionalComment { get; set; }

        public List<string> Likes { get; set; }
    }


    public enum ProductType
    {
        Drink,
        MainDish,
        Dessert,
        Appetizer

    }
}