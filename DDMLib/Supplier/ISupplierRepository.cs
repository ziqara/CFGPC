using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public interface ISupplierRepository
    {
        List<Supplier> ReadAllSuppliers();

        bool AddSupplier(Supplier supplier);

        bool existsByInn(int inn);

        bool existsByNameInsensitive(string name);

        bool existsByEmail(string email);

        bool existsOtherByNameInsensitive(string name, int currentInn);

        bool existsOtherByEmail(string email, int currentInn);

        bool UpdateSupplier(Supplier supplier);
    }
}
