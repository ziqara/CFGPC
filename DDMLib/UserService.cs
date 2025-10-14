using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDMLib
{
    public  class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public string RegisterUser(string email, string password, string passwordConfirm, string fullName, string phone = null, string address = null)
        {

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".") || email.IndexOf("@") >= email.LastIndexOf("."))
                return "Некорректный email";
            
            if (password != passwordConfirm)
                return "Пароли не совпадают";
            
            var existing = _userRepository.FindByEmail(email);
            if (existing != null)
                return "Email уже зарегистрирован";
            
            if (!string.IsNullOrWhiteSpace(phone))
            {
                bool isValidPhone = phone.All(c => char.IsDigit(c) || c == '+' || c == ' ' || c == '-' || c == '(' || c == ')');
                if (!isValidPhone || phone.Length < 5)
                    return "Неверный формат телефона";
            }
            
            var user = new User
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Phone = phone,
                Address = address,
                IsActive = true
            };
            _userRepository.Save(user);
            return "Аккаунт успешно создан!";
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

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, pattern))
            {
                return "Некорректный email";
            }

            try
            {

                User user = _userRepository.FindByEmail(email);
                if (user == null)
                {
                    return "Аккаунт не найден";
                }

                if (user.Password != password)
                {
                    return "Неверный пароль";
                }

                return string.Empty; 
            }
            catch (Exception ex)
            {
                return "Ошибка при поиске аккаунта";
            }
        }
    }
}
