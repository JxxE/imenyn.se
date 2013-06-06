using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantup.Data.Abstract
{
    public interface IAuthentication
    {
        void SignOut();
        void SetAuthCookie(string username, bool createPersistentCookie);
        void SetAuthCookie(string username, bool createPersistentCookie, string cookiePath);
    }
}
