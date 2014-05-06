using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Infrastructure;

namespace iMenyn.Data.Concrete.Db
{
    public class RavenDb : IDb
    {
        public RavenDb(IDbAccounts accounts = null, IDbEnterprises enterprises = null, IDbProducts products = null)
        {
            var documentStore = DependencyManager.DocumentStore;
            var logger = DependencyManager.Logger;

            Accounts = accounts ?? new RavenDbAccounts(documentStore, logger);
            Enterprises = enterprises ?? new RavenDbEnterprises(documentStore, logger);
            Products = products ?? new RavenDbProducts(documentStore, logger);
        }

        #region Implementation of IDb
        public IDbAccounts Accounts { get; private set; }
        public IDbEnterprises Enterprises { get; private set; }
        public IDbProducts Products { get; private set; }
        #endregion
    }
}