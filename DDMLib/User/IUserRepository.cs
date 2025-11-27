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
    }
}