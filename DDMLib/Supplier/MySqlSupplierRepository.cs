using System.Collections.Generic;
using MySql.Data.MySqlClient;
using DDMLib;

public class MySqlSupplierRepository : ISupplierRepository
{
    public bool AddSupplier(Supplier supplier)
    {
        throw new System.NotImplementedException();
    }

    public bool existsByEmail(string email)
    {
        throw new System.NotImplementedException();
    }

    public bool existsByInn(int inn)
    {
        throw new System.NotImplementedException();
    }

    public bool existsByNameInsensitive(string name)
    {
        throw new System.NotImplementedException();
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
}
