using System.Collections.Generic;

namespace iMenyn.Data.Models
{
    public class Menu
    {
        public string Id { get; set; }
        public IEnumerable<string> Products { get; set; }
    }
}