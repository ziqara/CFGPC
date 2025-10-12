using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return "Вход выполнен успешно!";
        }
    }
}
