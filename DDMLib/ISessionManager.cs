using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public interface ISessionManager
    {
        bool ValidateSession();
        string GetUserEmailFromSession();
        void CreateSession(string email, string userName);
        void InvalidateSession();
        bool IsUserAuthenticated();
    }
}
