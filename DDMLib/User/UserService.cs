// #nullable disable
using System;
using System.Data;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class UserService
    {
        private readonly IUserRepository userRepository_;
        private readonly SessionManager sessionManager_;

        public UserService(IUserRepository userRepository, SessionManager sessionManager)
        {
            userRepository_ = userRepository;
            sessionManager_ = sessionManager;
        }

        public string ValidateEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, emailPattern))
                return "Некорректный email. Требуемый формат: username@domain.com";
            return string.Empty;
        }

        public string ValidatePhone(string phone)
        {
            if (!string.IsNullOrWhiteSpace(phone))
            {
                string phonePattern = @"^\+?[\d\s-()]{7,}$";
                if (!Regex.IsMatch(phone, phonePattern))
                    return "Неверный формат телефона. Пример: +7 (999) 123-45-67";
            }
            return string.Empty;
        }

        public string ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "ФИО не может быть пустым";

            if (fullName.Length > 255)
                return "Превышена допустимая длина ФИО (≤ 255)";

            return string.Empty;
        }

        public string RegisterUser(User user, string passwordConfirm)
        {
            user.Email = user.Email == null ? null : user.Email.Trim();
            user.FullName = user.FullName == null ? null : user.FullName.Trim();
            user.Phone = user.Phone == null ? null : user.Phone.Trim();
            user.Address = user.Address == null ? null : user.Address.Trim();

            string emailError = ValidateEmail(user.Email);
            if (!string.IsNullOrEmpty(emailError)) return emailError;

            string phoneError = ValidatePhone(user.Phone);
            if (!string.IsNullOrEmpty(phoneError)) return phoneError;

            if (user.Password != passwordConfirm) return "Пароли не совпадают";

 
            var existing = userRepository_.FindByEmail(user.Email);
            if (existing != null) return "Email уже зарегистрирован";

            try
            {
                userRepository_.Save(user);
                return "Аккаунт успешно создан!";
            }
            catch (DuplicateNameException)
            {
                return "Email уже зарегистрирован";
            }
            catch
            {
                return "Ошибка при регистрации аккаунта.";
            }
        }

        public string LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email)) return "Введите email";
            if (string.IsNullOrWhiteSpace(password)) return "Введите пароль";

            string emailError = ValidateEmail(email);
            if (!string.IsNullOrEmpty(emailError)) return emailError;

            var user = userRepository_.FindByEmail(email);
            if (user == null) return "Аккаунт не найден";

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return "Неверный пароль";

            sessionManager_.CreateSession(user.Email, user.FullName);

            return string.Empty;
        }
    }
}
