using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iMenyn.Data.ViewModels
{
    public class MenuViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Id { get; set; }
        //public List<string> Features { get; set; } TODO

        public ProductListViewModel Products { get; set; }
        public bool RecentlyModified { get; set; }

        public string City { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }

        public object Categories { get; set; }

        public bool IsPremium { get; set; }
    }
}