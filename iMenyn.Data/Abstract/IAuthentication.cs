namespace iMenyn.Data.Abstract
{
    public interface IAuthentication
    {
        void SignOut();
        void SetAuthCookie(string username, bool createPersistentCookie);
        void SetAuthCookie(string username, bool createPersistentCookie, string cookiePath);
    }
}
