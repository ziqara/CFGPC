using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class SupplierService
    {
        private readonly ISupplierRepository _repo;

        public SupplierService(ISupplierRepository repo)
        {
            if (repo == null)
                throw new System.ArgumentNullException(nameof(repo));

            _repo = repo;
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _repo.ReadAllSuppliers();
        }
    }
}
