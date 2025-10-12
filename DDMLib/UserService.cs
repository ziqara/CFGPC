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
