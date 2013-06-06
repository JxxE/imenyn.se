using System.Collections.Generic;

namespace Rantup.Data.Models
{
    public class ModifiedMenu
    {
        public string Id { get; set; }
        public string EnterpriseId { get; set; }
        public List<string> ProductIds { get; set; }
    }
}