using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class SupplierService
    {
        private readonly ISupplierRepository repo_;

        public SupplierService(ISupplierRepository repo)
        {
            repo_ = repo;
        }

        public List<Supplier> GetAllSuppliers()
        {
            return repo_.ReadAllSuppliers();
        }
    }
}
