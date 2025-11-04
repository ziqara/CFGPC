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
            try
            {
                if (!sessionManager_.IsUserAuthenticated())
                    return "Требуется авторизация";

                if (string.IsNullOrWhiteSpace(fullName))
                    return "ФИО не может быть пустым";

                if (fullName.Length > 255)
                    return "Превышена допустимая длина ФИО (≤ 255)";

                if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhoneFormat(phone))
                    return "Неверный формат телефона. Пример: +7 (999) 123-45-67";

                string sessionEmail = sessionManager_.GetUserEmailFromSession();
                if (sessionEmail != email)
                    return "Доступ запрещён";

                User user = userRepository_.FindByEmail(email);
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
            if (!sessionManager_.IsUserAuthenticated())
                return "Требуется авторизация";

            if (newPassword != repeatPassword)
                return "Пароли не совпадают";

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return "Пароль недостаточно надёжный (минимум 6 символов)";

            string sessionEmail = sessionManager_.GetUserEmailFromSession();
            if (sessionEmail != email)
                return "Доступ запрещён";

            return string.Empty;
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        private bool IsValidPhoneFormat(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var regex = new Regex(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$");
            return regex.IsMatch(phone);
        }
    }
}
