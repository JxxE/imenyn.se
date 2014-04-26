using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Data.Abstract.Db
{
    public interface IDbAccounts
    {
        Account GetAccountByEmail(string email);

        void AddAccount(Account account);

        void UpdateAccount(Account account);

        IEnumerable<Account> GetAccounts();

        Account GetAccount(string accountId);
    }
}