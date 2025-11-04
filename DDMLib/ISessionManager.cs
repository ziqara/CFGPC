using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public interface ISessionManager
    {
        void CreateSession(string email, string userName);
        bool IsUserAuthenticated();
        string GetUserEmailFromSession();
        void InvalidateSession();
    }
}
