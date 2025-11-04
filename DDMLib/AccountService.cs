using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class AccountService
    {
        private readonly IUserRepository userRepository_;
        private readonly ISessionManager sessionManager_;

        public AccountService(IUserRepository userRepository, ISessionManager sessionManager)
        {
            userRepository_ = userRepository;
            sessionManager_ = sessionManager;
        }

        public User GetUserProfile(string email)
        {
            try
            {
                if (!sessionManager_.IsUserAuthenticated())
                    return null;

                if (string.IsNullOrWhiteSpace(email))
                    return null;

                string sessionEmail = sessionManager_.GetUserEmailFromSession();
                if (sessionEmail != email)
                    return null;

                User user = userRepository_.FindByEmail(email);

                if (user == null)
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("GetUserProfile", ex.Message);
                return null;
            }
        }

        public string UpdateProfile(string email, string fullName, string phone, string address)
        {
            throw new NotImplementedException();
        }

        public string ChangePassword(string email, string currentPassword, string newPassword, string repeatPassword)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
