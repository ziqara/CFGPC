// #nullable disable
using System;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using DDMLib;
using System.Data.SqlClient;

public class UserRepository : IUserRepository
{
    public User FindByEmail(string email)
    {
        if (!Config.TestDatabaseConnection())
        {
            ErrorLogger.LogError("FindByEmail", "Не удалось подключиться к базе данных.");
        }

        using (var connection = new MySqlConnection(Config.ConnectionString))
        {
            try
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    SELECT email, password_hash, full_name, phone, address
                    FROM users
                    WHERE email = @email
                    LIMIT 1;", connection);

                command.Parameters.AddWithValue("@email", email == null ? (object)DBNull.Value : email.Trim());

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null; 

                    int iEmail = reader.GetOrdinal("email");
                    int iPass = reader.GetOrdinal("password_hash");
                    int iFull = reader.GetOrdinal("full_name");
                    int iPhone = reader.GetOrdinal("phone");
                    int iAddr = reader.GetOrdinal("address");

                    Func<int, string> GetStringOrEmpty = i => reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                    Func<int, string> GetStringOrNull = i => reader.IsDBNull(i) ? null : reader.GetString(i);

                    return new User
                    {
                        Email = GetStringOrEmpty(iEmail),
                        Password = GetStringOrEmpty(iPass),
                        FullName = GetStringOrEmpty(iFull),
                        Phone = GetStringOrNull(iPhone),
                        Address = GetStringOrNull(iAddr)
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
        if (!Config.TestDatabaseConnection())
        {
            ErrorLogger.LogError("Save", "Не удалось подключиться к базе данных.");
        }

        using (var connection = new MySqlConnection(Config.ConnectionString))
        {
            try
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    INSERT INTO users (email, password_hash, full_name, phone, address)
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

            string sql = @"UPDATE users SET full_name = @FullName, phone = @Phone, address = @Address WHERE email = @Email";

            using (var connection = new SqlConnection(Config.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
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

        catch(Exception ex)
        {
            ErrorLogger.LogError("UpdateProfile", ex.Message);
            return "Ошибка при обновлении профиля";
        }
    }

    public bool UpdatePasswordHash(string email, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            string newPasswordHash = HashPassword(newPassword);

            string sql = @"UPDATE users SET password = @PasswordHash WHERE email = @Email";

            using (var connection = new SqlConnection(Config.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
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
            return false;
        }
    }

    public bool VerifyPassword(User user, string password)
    {
        throw new NotImplementedException();
    }
}
