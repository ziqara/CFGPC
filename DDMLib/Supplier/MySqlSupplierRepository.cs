using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;
using System;

public class MySqlSupplierRepository : ISupplierRepository
{
    public bool AddSupplier(Supplier supplier)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();

                string sql = @"INSERT INTO suppliers (inn, name, contactEmail, phone, address)
                               VALUES (@inn, @name, @mail, @phone, @addr);";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@inn", supplier.Inn);
                    cmd.Parameters.AddWithValue("@name", supplier.Name);
                    cmd.Parameters.AddWithValue("@mail", supplier.ContactEmail);

                    cmd.Parameters.AddWithValue("@phone", supplier.Phone == null ? (object)DBNull.Value : supplier.Phone);
                    cmd.Parameters.AddWithValue("@addr", supplier.Address == null ? (object)DBNull.Value : supplier.Address);

                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool existsByEmail(string email)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM suppliers WHERE contactEmail = @mail;";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@mail", email);

                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool existsByInn(int inn)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM suppliers WHERE inn = @inn;";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@inn", inn);

                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool existsByNameInsensitive(string name)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM suppliers WHERE LOWER(name) = LOWER(@name);";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);

                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool existsOtherByEmail(string email, int currentInn)
    {
        throw new NotImplementedException();
    }

    public bool existsOtherByNameInsensitive(string name, int currentInn)
    {
        throw new NotImplementedException();
    }

    public List<Supplier> ReadAllSuppliers()
    {
        List<Supplier> suppliers = new List<Supplier>();

        try
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT inn, name, contactEmail, phone, address FROM suppliers;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Supplier s = new Supplier(reader.GetInt32(0))
                        {
                            Name = reader.GetString(1),
                            ContactEmail = reader.GetString(2),
                            Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Address = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };

                        suppliers.Add(s);
                    }
                }
            }

            return suppliers;
        }
        catch
        {
            throw; // ошибка пробрасывается в ui
        }
    }

    public bool UpdateSupplier(Supplier supplier)
    {
        if (supplier == null)
            throw new ArgumentNullException("supplier");

        try
        {
            using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
            {
                connection.Open();

                const string sql = @"
                    UPDATE suppliers
                    SET
                        name         = @name,
                        contactEmail = @mail,
                        phone        = @phone,
                        address      = @addr
                    WHERE inn = @inn;";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@inn", supplier.Inn);
                    cmd.Parameters.AddWithValue("@name", supplier.Name == null ? (object)DBNull.Value : (object)supplier.Name.Trim());
                    cmd.Parameters.AddWithValue("@mail", supplier.ContactEmail == null ? (object)DBNull.Value : (object)supplier.ContactEmail.Trim());

                    if (string.IsNullOrWhiteSpace(supplier.Phone))
                        cmd.Parameters.AddWithValue("@phone", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@phone", supplier.Phone.Trim());

                    if (string.IsNullOrWhiteSpace(supplier.Address))
                        cmd.Parameters.AddWithValue("@addr", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@addr", supplier.Address.Trim());

                    int rows = cmd.ExecuteNonQuery();
                    // если обновили ровно одну строку — успех
                    return rows == 1;
                }
            }
        }
        catch
        {
            // для сервиса: UpdateSupplier вернул false → "Не удалось сохранить изменения..."
            return false;
        }
    }
}
