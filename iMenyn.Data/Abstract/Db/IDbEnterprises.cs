﻿using System.Collections.Generic;
using iMenyn.Data.Models;
using iMenyn.Data.ViewModels;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbEnterprises
    {
        void UpdateEnterprise(Enterprise enterprise, List<Product> products);

        string CreateEnterprise(Enterprise enterprise);

        void DeleteEnterpriseById(string enterpriseId);

        IEnumerable<Enterprise> SearchEnterprises(string searchTerm, string location, string categorySearch);

        IEnumerable<Enterprise> CheckIfEnterpriseExists(string key, int postalCode);

        IEnumerable<Enterprise> GetAllEnterprises();

        IEnumerable<Enterprise> GetNewEnterprises();

        Enterprise GetEnterpriseById(string enterpriseId);
        CompleteEnterpriseViewModel GetCompleteEnterprise(string enterpriseId);

        IEnumerable<string> GetModifiedMenus();

        Enterprise GetEnterpriseByUrlKey(string urlKey);

        IEnumerable<Enterprise> GetEnterprisesByLocation(string stateCode, string city);
    }
}