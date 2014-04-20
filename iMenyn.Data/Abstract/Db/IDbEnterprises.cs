using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbEnterprises
    {
        void UpdateEnterprise(Enterprise enterprise);

        string CreateEnterprise(Enterprise enterprise);

        void DeleteEnterpriseById(string enterpriseId);

        IEnumerable<Enterprise> SearchEnterprises(string searchTerm, string location, string categorySearch);

        IEnumerable<Enterprise> CheckIfEnterpriseExists(string key, int postalCode);

        IEnumerable<Enterprise> GetAllEnterprises();

        IEnumerable<Enterprise> GetNewEnterprises();

        Enterprise GetEnterpriseById(string enterpriseId);

        IEnumerable<Enterprise> GetEnterprisesWithModifiedMenus();

        Enterprise GetEnterpriseByUrlKey(string urlKey);

        IEnumerable<Enterprise> GetEnterprisesByLocation(string stateCode, string city);
    }
}