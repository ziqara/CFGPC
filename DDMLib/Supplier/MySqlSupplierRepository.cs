using System;
using System.Collections.Generic;
using DDMLib;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public class MySqlSupplierRepository : ISupplierRepository
    {
        public List<Supplier> ReadAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();
        }
    }
}
