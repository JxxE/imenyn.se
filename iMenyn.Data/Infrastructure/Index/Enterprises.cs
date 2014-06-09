using System.Linq;
using iMenyn.Data.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace iMenyn.Data.Infrastructure.Index
{
    public class Enterprises : AbstractIndexCreationTask<Enterprise>
    {
        public Enterprises()
        {
            Map = enterprises => from enterprise in enterprises
                                 select new
                                            {
                                                enterprise.Name,
                                                enterprise.PostalCode,
                                                enterprise.Categories,
                                                enterprise.Coordinates,
                                                enterprise.CountryCode,
                                                enterprise.Id,
                                                enterprise.IsNew,
                                                enterprise.LockedFromEdit,
                                                enterprise.LastUpdated,
                                                enterprise.Menu,
                                                enterprise.Phone,
                                                _ = SpatialGenerate("Coordinates", enterprise.Coordinates.Lat, enterprise.Coordinates.Lng)
                                            };

            //Reduce = results => from result in results
            //                    group result by result.Id into g
            //                    select new
            //                               {
                                               
            //                                   g.First().Name,
            //                                   g.First().StateCode,
            //                                   g.First().Key,
            //                                   g.First().City,
            //                                   g.First().Address,
            //                                   g.First().PostalCode,
            //                                   g.First().YelpId,
            //                                   g.First().Categories,
            //                                   g.First().Coordinates,
            //                                   g.First().CountryCode,
            //                                   g.First().Id,
            //                                   g.First().IsPremium,
            //                                   g.First().IsTemp,
            //                                   g.First().LastUpdated,
            //                                   g.First().Menu,
            //                                   g.First().Phone
            //                               };


            Indexes.Add(x => x.Name, FieldIndexing.Analyzed);
            Indexes.Add(x=>x.PostalCode, FieldIndexing.Analyzed);
            Indexes.Add(x => x.IsNew, FieldIndexing.Default);
            Indexes.Add(x => x.LockedFromEdit, FieldIndexing.Default);
            Indexes.Add(x=>x.Categories,FieldIndexing.Default);
        }
    }
}