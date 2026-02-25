using System.Collections.Generic;

namespace DDMLib
{
    public interface IStorageRepository
    {
        List<Storage> ReadAllStorages();

        bool AddStorage(Storage storage);

        bool ExistsSameStorage(string name, string brand, string model);

        bool UpdateStorage(Storage storage);

        bool ExistsOtherSameStorage(string name, string brand, string model, int currentComponentId);

        bool DeleteById(int componentId);

        bool HasActiveOrders(int componentId);
    }
}