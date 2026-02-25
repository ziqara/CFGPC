// #nullable disable
using System;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using DDMLib;
using System.Collections.Generic;

public class UserRepository : IUserRepository
{
    public User FindByEmail(string email)
    {
        if (!Config.TestDatabaseConnection())
        {
            ErrorLogger.LogError("FindByEmail", "Не удалось подключиться к базе данных.");
        }

        using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(@"
                    SELECT email, passwordHash, fullName, phone, address, registrationDate, avatar
                    FROM users
                    WHERE email = @email
                    LIMIT 1;", connection);

                command.Parameters.AddWithValue("@email", email == null ? (object)DBNull.Value : email.Trim());

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    int iEmail = reader.GetOrdinal("email");
                    int iPass = reader.GetOrdinal("passwordHash");
                    int iFull = reader.GetOrdinal("fullName");
                    int iPhone = reader.GetOrdinal("phone");
                    int iAddr = reader.GetOrdinal("address");
                    int iRegDate = reader.GetOrdinal("registrationDate");
                    int iAvatar = reader.GetOrdinal("avatar");

                    Func<int, string> GetStringOrEmpty = i => reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                    Func<int, string> GetStringOrNull = i => reader.IsDBNull(i) ? null : reader.GetString(i);
                    Func<int, DateTime> GetDateTimeOrMin = i => reader.IsDBNull(i) ? DateTime.MinValue : reader.GetDateTime(i);
                    Func<int, byte[]> GetBlobOrNull = i => reader.IsDBNull(i) ? null : (byte[])reader.GetValue(i);

                    return new User
                    {
                        Email = GetStringOrEmpty(iEmail),
                        Password = GetStringOrEmpty(iPass),
                        FullName = GetStringOrEmpty(iFull),
                        Phone = GetStringOrNull(iPhone),
                        Address = GetStringOrNull(iAddr),
                        RegistrationDate = GetDateTimeOrMin(iRegDate),
                        Avatar = GetBlobOrNull(iAvatar)
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("FindByEmail", ex.Message);
                throw;
            }
        }
    }

    public User Save(User user)
    {
        using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(@"
                    INSERT INTO users (email, passwordHash, fullName, phone, address)
                    VALUES (@email, @password_hash, @full_name, @phone, @address);", connection);

                command.Parameters.AddWithValue("@email", user.Email == null ? (object)DBNull.Value : user.Email.Trim());
                command.Parameters.AddWithValue("@password_hash", HashPassword(user.Password));
                command.Parameters.AddWithValue("@full_name", (object)(user.FullName == null ? null : user.FullName.Trim()) ?? DBNull.Value);
                command.Parameters.AddWithValue("@phone", (object)user.Phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@address", (object)user.Address ?? DBNull.Value);

                command.ExecuteNonQuery();
                return user;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new DuplicateNameException("Email уже зарегистрирован");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Save", ex.Message);
                throw;
            }
        }
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string UpdateProfile(User user)
    {
        try
        {
            User existingUser = FindByEmail(user.Email);
            if (existingUser == null)
                return "Пользователь не найден";

            string sql = @"UPDATE users SET fullName = @FullName, phone = @Phone, address = @Address WHERE email = @Email";

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    return "Не удалось обновить профиль. Пользователь не найден.";
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError("UpdateProfile", ex.Message);
            return "Ошибка при обновлении профиля: " + ex.Message;
        }
    }

    public bool UpdatePasswordHash(string email, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            string newPasswordHash = HashPassword(newPassword);

            string sql = @"UPDATE users SET passwordHash = @PasswordHash WHERE email = @Email";

            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError("UpdatePasswordHash", ex.Message);
            return false;
        }
    }

    public bool VerifyPassword(User user, string password)
    {
        try
        {
            if (user == null || string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(user.Password))
                return false;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return isPasswordValid;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError("VerifyPassword", ex.Message);
            return false;
        }
    }

    public bool UpdateAvatar(string email, byte[] avatarData)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email не может быть пустым.", nameof(email));
        }

        using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
        {
            try
            {
                connection.Open();

                string query = @"
                    UPDATE users
                    SET avatar = @avatar
                    WHERE email = @email";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@avatar", (object)avatarData ?? DBNull.Value);
                command.Parameters.AddWithValue("@email", email);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("UserRepository UpdateAvatar", ex.Message);
                throw;
            }
        }
    }

    public List<User> ReadAllUsers()
    {
        var users = new List<User>();

        using (var connection = new MySqlConnection(Config.ConnectionString))
        {
            connection.Open();

            const string sql = @"
                SELECT email, passwordHash, fullName, phone, address, registrationDate, avatar
                FROM users
                ORDER BY registrationDate DESC;";

            using (var command = new MySqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Email = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Password = reader.IsDBNull(1) ? "" : reader.GetString(1), // passwordHash
                        FullName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                        RegistrationDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                        Avatar = reader.IsDBNull(6) ? null : (byte[])reader["avatar"]
                    });
                }
            }
        }

        return users;
    }

    public Dictionary<string, bool> ReadActiveOrdersFlags()
    {
        var map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        using (var connection = new MySqlConnection(Config.ConnectionString))
        {
            connection.Open();

            // активные = НЕ delivered/cancelled
            const string sql = @"
                SELECT u.email,
                       EXISTS(
                           SELECT 1
                           FROM orders o
                           WHERE o.userEmail = u.email
                             AND o.status NOT IN ('delivered','cancelled')
                       ) AS hasActive
                FROM users u;";

            using (var cmd = new MySqlCommand(sql, connection))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    string email = r.IsDBNull(0) ? "" : r.GetString(0);
                    bool hasActive = Convert.ToInt32(r.GetValue(1)) == 1;
                    map[email] = hasActive;
                }
            }
        }

        return map;
    }

    public bool HasAnyOrders(string email)
    {
        using (var connection = new MySqlConnection(Config.ConnectionString))
        {
            connection.Open();

            const string sql = "SELECT COUNT(*) FROM orders WHERE userEmail=@email;";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@email", email);
                return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
            }
        }
    }

    public bool DeleteByEmail(string email)
    {
        using (var connection = new MySqlConnection(Config.ConnectionString))
        {
            connection.Open();

            const string sql = "DELETE FROM users WHERE email=@email;";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@email", email);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}