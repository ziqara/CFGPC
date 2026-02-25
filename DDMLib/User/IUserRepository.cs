using System.Collections.Generic;

namespace DDMLib
{
    public interface IUserRepository
    {
        User FindByEmail(string email);
        User Save(User user);
        string UpdateProfile(User user);
        bool UpdatePasswordHash(string email, string newPassword);
        bool VerifyPassword(User user, string password);
        bool UpdateAvatar(string email, byte[] avatarData);

        // ADMIN
        List<User> ReadAllUsers();
        Dictionary<string, bool> ReadActiveOrdersFlags(); // email -> есть активные заказы
        bool DeleteByEmail(string email);
        bool HasAnyOrders(string email);
    }
}