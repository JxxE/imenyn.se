using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Models;

namespace iMenyn.Data.Concrete.Db
{
    public class RavenDbAccounts : IDbAccounts
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger _logger;

        public RavenDbAccounts(IDocumentStore documentStore, ILogger logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public Account GetACcountByEmail(string email)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Account>().FirstOrDefault(u => u.Email == email);
            }
        }

        public void AddAccount(Account account)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(account);
                session.SaveChanges();
                _logger.Info("Created account " + account.Name);
            }
        }

        public void UpdateAccount(Account account)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(account);
                session.SaveChanges();
                _logger.Info("Updated account " + account.Name);
            }
        }

        public IEnumerable<Account> GetAccounts()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Account>();
            }
        }

        public Account GetAccount(string accountId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<Account>(accountId);
            }
        }
    }
}