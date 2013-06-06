using System.Collections.Generic;

namespace Rantup.Data.Models
{
    public class Menu
    {
        public string Id { get; set; }
        public IEnumerable<string> Products { get; set; }
    }
}