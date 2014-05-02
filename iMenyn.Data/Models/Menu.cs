using System.Collections.Generic;

namespace iMenyn.Data.Models
{
    public class Menu
    {
        public List<Category> Categories { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Products { get; set; }
    }

    public class ModifiedMenu
    {
        public string Id { get; set; }
        public Menu Menu { get; set; }
    }
}