using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class MySqlSupplierRepository : ISupplierRepository
    {
        public List<Supplier> ReadAllSuppliers()
        {
            var suppliers = new List<Supplier>();

            if (!Config.TestDatabaseConnection())
            {
                ErrorLogger.LogError(nameof(ReadAllSuppliers), "Не удалось подключиться к базе данных.");
            }

            using (var connection = new MySqlConnection(Config.ConnectionString))
            {
                try
                {
                    connection.Open();

                    var command = new MySqlCommand(@"SELECT inn, name, contact_email, phone, address FROM suppliers ORDER BY name;", connection);

                    using (var reader = command.ExecuteReader())
                    {
                        int iInn = reader.GetOrdinal("inn");
                        int iName = reader.GetOrdinal("name");
                        int iEmail = reader.GetOrdinal("contact_email");
                        int iPhone = reader.GetOrdinal("phone");
                        int iAddress = reader.GetOrdinal("address");

                        while (reader.Read())
                        {
                            suppliers.Add(new Supplier
                            {
                                Inn = reader.IsDBNull(iInn) ? 0 : reader.GetInt32(iInn),
                                Name = reader.IsDBNull(iName) ? string.Empty : reader.GetString(iName),
                                ContactEmail = reader.IsDBNull(iEmail) ? string.Empty : reader.GetString(iEmail),
                                Phone = reader.IsDBNull(iPhone) ? null : reader.GetString(iPhone),
                                Address = reader.IsDBNull(iAddress) ? null : reader.GetString(iAddress)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(nameof(ReadAllSuppliers), ex.Message);
                    throw;
                }
            }

            return suppliers;
        }
    }
}
