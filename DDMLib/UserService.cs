using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDMLib
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex PhoneRegex = new Regex(
            @"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$", RegexOptions.Compiled);
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string RegisterUser(
        string email,
        string password,
        string passwordConfirm,
        string fullName,
        string phone = null,
        string address = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                {
                    return "Некорректный email";
                }
                // Проверка совпадения паролей
                if (password != passwordConfirm)
                {
                    return "Пароли не совпадают";
                }

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    return "ФИО не может быть пустым";
                }

                var existingUser = _userRepository.FindByEmail(email);
                if (existingUser != null)
                {
                    return "Email уже зарегистрирован";
                }

                if (!string.IsNullOrEmpty(phone) && !PhoneRegex.IsMatch(phone))
                {
                    return "Неверный формат телефона";
                }

                var user = new User
                {
                    Email = email,
                    Password = password,
                    FullName = fullName,
                    Phone = phone,
                    Address = address,
                    RegistrationDate = DateTime.UtcNow,
                    IsActive = true
                };
                // Сохраняем пользователя
                var savedUser = _userRepository.Save(user);
                return "Аккаунт успешно создан!";
            }

            catch (Exception ex)
            {
                return "Ошибка сохранения в БД";
            }
        }
    }
}
