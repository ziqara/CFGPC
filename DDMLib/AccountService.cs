using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace DDMLib
{
    public class AccountService
    {
        private readonly IUserRepository userRepository_;
        private readonly SessionManager sessionManager_;
        private readonly UserService userService_;

        public AccountService(IUserRepository userRepository, SessionManager sessionManager, UserService userService)
        {
            userRepository_ = userRepository;
            sessionManager_ = sessionManager;
            userService_ = userService;
        }

        public User GetUserProfile(string email)
        {
            try
            {
                string authError = userService_.CheckAuthenticationAndAccess(email);
                if (!string.IsNullOrEmpty(authError))
                    return null;

                User user = userService_.ValidateUserExists(email);

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
            try
            {
                string authError = userService_.CheckAuthenticationAndAccess(email);
                if (!string.IsNullOrEmpty(authError))
                    return authError;

                string fullNameError = userService_.ValidateFullName(fullName);
                if (!string.IsNullOrEmpty(fullNameError))
                    return fullNameError;

                string phoneError = userService_.ValidatePhone(phone);
                if (!string.IsNullOrEmpty(phoneError))
                    return phoneError;

                User user = userService_.ValidateUserExists(email);
                if (user == null)
                    return "Пользователь не найден";

                user.FullName = fullName.Trim();
                user.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
                user.Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();

                string updateResult = userRepository_.UpdateProfile(user);
                if (!string.IsNullOrEmpty(updateResult))
                    return "Ошибка сохранения";

                return "Профиль обновлён";
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("UpdateProfile", ex.Message);
                return "Ошибка сохранения";
            }
        }

        public string ChangePassword(string email, string currentPassword, string newPassword, string repeatPassword)
        {
            try
            {
                string authError = userService_.CheckAuthenticationAndAccess(email);
                if (!string.IsNullOrEmpty(authError))
                    return authError;

                if (newPassword != repeatPassword)
                    return "Пароли не совпадают";

                string passwordError = userService_.ValidatePassword(newPassword);
                if (!string.IsNullOrEmpty(passwordError))
                    return passwordError;

                User user = userService_.ValidateUserExists(email);
                if (user == null)
                    return "Пользователь не найден";

                if (!userRepository_.VerifyPassword(user, currentPassword))
                    return "Неверный пароль";

                bool updateResult = userRepository_.UpdatePasswordHash(email, newPassword);
                if (!updateResult)
                    return "Ошибка сохранения";

                return "Пароль обновлён";
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("ChangePassword", ex.Message);
                return "Ошибка сохранения";
            }
        }

        public void Logout()
        {
            try
            {
                sessionManager_.InvalidateSession();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Logout", ex.Message);
            }
        }
    }
}
