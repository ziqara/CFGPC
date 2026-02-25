using System;
using System.Collections.Generic;

namespace DDMLib
{
    public class AdminUserService
    {
        private readonly IUserRepository repo_;

        public AdminUserService(IUserRepository repo)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public List<User> GetAllUsers()
        {
            return repo_.ReadAllUsers();
        }

        public string UpdateAvatar(string email, byte[] avatarBytes)
        {
            try
            {
                bool ok = repo_.UpdateAvatar(email, avatarBytes);
                return ok ? string.Empty : "Не удалось обновить аватар";
            }
            catch (Exception ex)
            {
                return "Ошибка при обновлении аватара: " + ex.Message;
            }
        }
        public Dictionary<string, bool> GetActiveOrdersFlags()
        {
            return repo_.ReadActiveOrdersFlags();
        }

        // Используем существующую логику обновления профиля
        public string UpdateProfile(User user)
        {
            return repo_.UpdateProfile(user);
        }

        public string DeleteUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Email не задан";

            if (repo_.HasAnyOrders(email))
                return "Невозможно удалить: у пользователя есть заказы";

            bool ok = repo_.DeleteByEmail(email);
            return ok ? string.Empty : "Запись не найдена";
        }
    }
}