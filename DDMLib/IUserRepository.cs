namespace DDMLib
{
    public interface IUserRepository
    {
        User FindByEmail(string email);
        User Save(User user);
        string UpdateProfile(User user);
        bool UpdatePasswordHash(User user, string newPassword);
        bool VerifyPassword(User user, string password);
    }
}