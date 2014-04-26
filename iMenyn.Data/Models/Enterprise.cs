using System;
using System.Collections.Generic;

namespace iMenyn.Data.Models
{
    public class Enterprise
    {
        public string Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }

        // Gatuadress: Ringvägen
        public string StreetRoute { get; set; }

        // Gatunummer: 2A
        public string StreetNumber { get; set; }

        public int PostalCode { get; set; }

        // Postort: Mariefred, Grödinge, Stockholm
        public string PostalTown { get; set; }

        // Kommun: Botkyrka
        public string Commune { get; set; }

        // Län: Stockholms län, Södermanlands län
        public string County { get; set; }

        //Norsborg, Södermalm
        public string SubLocality { get; set; }


        public string CountryCode { get; set; }


        public Coordinates Coordinates { get; set; }


        public List<string> Categories { get; set; }


        public List<string> SearchTags { get; set; }

        public bool IsPremium { get;set;}
        public bool IsNew { get; set; }
        public bool LockedFromEdit { get; set; }

        public DateTime LastUpdated { get; set; }

        public Menu Menu { get; set; }


        //[Obsolete]
        //public string StateCode { get; set; }
        //[Obsolete]
        //public string City { get; set; }
        //[Obsolete]
        //public string YelpId { get; set; }
        //[Obsolete]
        //public string Address { get; set; }
    }

    public class Coordinates
    {
        public string Lng { get; set; }
        public string Lat { get; set; }
    }
}