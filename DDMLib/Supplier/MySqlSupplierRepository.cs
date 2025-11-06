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
                }
            }
            catch 
            {
                
            }
    }
}
