using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public interface IOrderRepository
    {
        List<OrderCard> GetUserOrders(string userEmail);
    }
}
