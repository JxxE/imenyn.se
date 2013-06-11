namespace Rantup.Data.Helpers
{
    public class AccountHelper
    {
        public static string GetId(string email)
        {
            return "Accounts-" + email;
        }
    }
}