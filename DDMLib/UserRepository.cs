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
                            FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString("full_name"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                            Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString("address"),
                        };
                    }
                    return null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByEmail: {ex.Message}");
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
                var checkCommand = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email = @email", connection);
                checkCommand.Parameters.AddWithValue("@email", user.Email);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count == 0)
                {
                    var command = new MySqlCommand(
                        "INSERT INTO users (email, password_hash, full_name, phone, address, role, is_active) " +
                        "VALUES (@email, @password_hash, @full_name, @phone, @address, 'client', @is_active)",
                        connection);

                    command.Parameters.AddWithValue("@password_hash", HashPassword(user.Password));
                    command.Parameters.AddWithValue("@full_name", user.FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@address", user.Address ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();

                    return user;  
                }
                else
                {
                    throw new Exception("Пользователь с таким email уже существует.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Save: {ex.Message}");
                throw;
            }
        }
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password); 
    }
}