using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Order
{
    public interface IOrderRepository
    {
        List<Order> GetOrdersByUserEmail(string userEmail);
        void AddOrder(Order order);

        // ADMIN
        List<Order> GetAllOrders();
        bool ExistsById(int orderId);
        bool UpdateOrderStatusAndPaid(int orderId, OrderStatus status, bool isPaid);
    }
}
