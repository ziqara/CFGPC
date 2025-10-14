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

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Метод для проверки формата email и телефона
        public string ValidateContactInfo(string email, string phone = null)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return "Некорректный email";
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                string phonePattern = @"^\+?[\d\s-()]{7,}$";
                if (!Regex.IsMatch(phone, phonePattern))
                {
                    return "Неверный формат телефона";
                }
            }

            return string.Empty;
        }

        public string RegisterUser(User user, string passwordConfirm)
        {
            
            string validationError = ValidateContactInfo(user.Email, user.Phone);
            if (!string.IsNullOrEmpty(validationError))
            {
                return validationError;  // Вернем ошибку, если формат неверный
            }

            if (user.Password != passwordConfirm)
            {
                return "Пароли не совпадают";
            }

            User existing = _userRepository.FindByEmail(user.Email);
            if (existing != null)
            {
                return "Email уже зарегистрирован";
            }

            try
            {
                user.IsActive = true;
                _userRepository.Save(user);  
                return "Аккаунт успешно создан!";
            }
            catch (Exception ex)
            {
                
                return "Ошибка при регистрации аккаунта.";
            }
        }

        public string LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Введите email";
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return "Введите пароль";
            }

           
            string validationError = ValidateContactInfo(email);
            if (!string.IsNullOrEmpty(validationError))
            {
                return validationError; 
            }

                User user = _userRepository.FindByEmail(email);
                if (user == null)
                {
                    return "Аккаунт не найден";
                }
                if (user.Password != password)
                {
                    return "Неверный пароль";
                }
                return string.Empty;  // Успешный логин
        }
    }
}