using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DDMLib;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class MySqlSupplierRepository : ISupplierRepository
    {
        public List<Supplier> ReadAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Config.ConnectionString))
                {
                    connection.Open();

                    string sql = "SELECT inn, name, contact_email, phone, address FROM suppliers";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Supplier s = new Supplier(reader.GetInt32(0));
                            s.Name = reader.GetString(1);
                            s.ContactEmail = reader.GetString(2);
                            s.Phone = reader.IsDBNull(3) ? null : reader.GetString(3);
                            s.Address = reader.IsDBNull(4) ? null : reader.GetString(4);
                            suppliers.Add(s);
                        }
                    }
                }
            }
            catch 
            {
                
            }
    }
}
