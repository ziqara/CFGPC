using System;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using DDMLib;

public class UserRepository : IUserRepository
{
    private string connectionString => Config.ConnectionString;

    public User FindByEmail(string email)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new MySqlCommand("SELECT email, password_hash, full_name, phone, address FROM users WHERE email = @email", connection);
                command.Parameters.AddWithValue("@email", email);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password_hash"), 
                            FullName = reader.GetString("full_name"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"), //GetOrdinal - для проверки на Null
                            Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString("address"),
                        };
                    }
                    return null; 
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
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO users (email, password_hash, full_name, phone, address) " +
                    "VALUES (@email, @password_hash, @full_name, @phone, @address)",
                    connection);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password_hash", HashPassword(user.Password));
                command.Parameters.AddWithValue("@full_name", user.FullName);
                command.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@address", user.Address ?? (object)DBNull.Value);
                command.ExecuteNonQuery();

                return user;

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
}