using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbEnterprises
    {
        void UpdateEnterprise(Enterprise enterprise);
        bool UpdateEnterprise(string enterpriseId, Menu menu);

        string CreateEnterprise(Enterprise enterprise);

        IEnumerable<Enterprise> SearchEnterprises(string searchTerm, string location, string categorySearch);

        IEnumerable<Enterprise> GetNearbyEnterprises(string lat, string lng);
        
        IEnumerable<Enterprise> CheckIfEnterpriseExists(string key, int postalCode);

        IEnumerable<Enterprise> GetAllEnterprises();

        IEnumerable<Enterprise> GetModifiedAndNewEnterprises();

        Enterprise GetEnterpriseById(string enterpriseId);
        CompleteEnterpriseViewModel GetCompleteEnterprise(string enterpriseId,bool edit=false);

        IEnumerable<Enterprise> GetModifiedEnterprises();

        Enterprise GetEnterpriseByUrlKey(string urlKey);

        IEnumerable<Enterprise> GetEnterprisesByLocation(string stateCode, string city);

        void DeleteEnterprise(string enterpriseId);

        void SetModifiedMenuAsDefault(string enterpriseId);

        void DisapproveModifiedMenu(string enterpriseId);
    }
}