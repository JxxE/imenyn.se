namespace iMenyn.Data.Abstract.Db
{
    public interface IDb
    {
        IDbAccounts Accounts { get; }

        IDbEnterprises Enterprises { get; }

        IDbProducts Products { get; }
    }
}