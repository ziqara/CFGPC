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


        bool UpdateSupplier(Supplier supplier);

        bool existsOtherByNameInsensitive(string name, int currentInn);

        bool existsOtherByEmail(string email, int currentInn);

        bool DeleteByInn(int inn);

        bool HasActiveOrders(int inn);
    }
}
