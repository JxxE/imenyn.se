using System;
using System.Collections.Generic;

namespace Rantup.Data.Models
{
    public class Enterprise
    {
        public string Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public Coordinates Coordinates { get; set; }
        
        public string YelpId { get; set; }

        public string Menu { get; set; }
        public List<string> Categories { get; set; }

        public bool IsPremium { get;set;}
        public bool IsTemp { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class Coordinates
    {
        public string Lng { get; set; }
        public string Lat { get; set; }
    }
}