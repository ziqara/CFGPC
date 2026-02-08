using System;
using System.Collections.Generic;
using DDMLib.Order;

namespace DDMLib.Order
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        // Конструктор для инъекции зависимости
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public List<Order> GetOrdersByUserEmail(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail))
                    throw new ArgumentException("Email пользователя не может быть пустым", nameof(userEmail));

                return _orderRepository.GetOrdersByUserEmail(userEmail);
            }
            catch
            {
                throw; // ошибка пробрасывается в ui
            }
        }
    }
}