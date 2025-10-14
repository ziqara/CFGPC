using System;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;

public class UserRepository : IUserRepository
{
    private string connectionString = "server=localhost;user=root;password=your_password;database=pc_store;";  // Замените на реальные данные

    public User FindByEmail(string email)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM users WHERE email = @email", connection);
                command.Parameters.AddWithValue("@email", email);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32("user_id"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password_hash"),  // Возвращаем хэш, но в сервисе сравниваем хэшированный пароль
                            FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString("full_name"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                            Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString("address"),
                            IsActive = reader.GetBoolean("is_active")
                            // Добавьте другие свойства, если нужно
                        };
                    }
                    return null;  // Пользователь не найден
                }
            }
            catch (Exception ex)
            {
                // Логируйте ошибку, если нужно
                Console.WriteLine($"Error in FindByEmail: {ex.Message}");
                throw;  // Пробросьте исключение для обработки в сервисе
            }
        }
    }

    public void Save(User user)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                // Проверка на существующий email (хотя в сервисе это уже делается)
                var checkCommand = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email = @email", connection);
                checkCommand.Parameters.AddWithValue("@email", user.Email);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count == 0)  // Только для новых пользователей
                {
                    var command = new MySqlCommand(
                        "INSERT INTO users (email, password_hash, full_name, phone, address, role, is_active) " +
                        "VALUES (@email, @password_hash, @full_name, @phone, @address, 'client', @is_active)",
                        connection);

                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@password_hash", HashPassword(user.Password));  // Хэшируем пароль
                    command.Parameters.AddWithValue("@full_name", user.FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@address", user.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@is_active", user.IsActive);

                    command.ExecuteNonQuery();
                }
                else
                {
                    throw new Exception("Пользователь с таким email уже существует.");  // Или обработайте по-другому
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Save: {ex.Message}");
                throw;  // Пробросьте для обработки
            }
        }
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);  // Хэширование с солью
    }
}