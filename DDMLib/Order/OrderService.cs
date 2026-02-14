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

        public void CreateOrder(Order order)
        {
            try
            {
                // Валидация обязательных полей
                if (order == null)
                    throw new ArgumentNullException(nameof(order), "Заказ не может быть пустым");

                if (order.ConfigId <= 0)
                    throw new ArgumentException("ID конфигурации должен быть указан", nameof(order.ConfigId));

                if (string.IsNullOrWhiteSpace(order.UserEmail))
                    throw new ArgumentException("Email пользователя не может быть пустым", nameof(order.UserEmail));

                if (order.TotalPrice <= 0)
                    throw new ArgumentException("Сумма заказа должна быть больше 0", nameof(order.TotalPrice));

                // Проверка адреса доставки: обязательна только при курьерской доставке
                if (order.DeliveryMethod == DeliveryMethod.Courier && string.IsNullOrWhiteSpace(order.DeliveryAddress))
                    throw new ArgumentException("Адрес доставки обязателен при курьерской доставке", nameof(order.DeliveryAddress));

                // Устанавливаем значения по умолчанию
                order.OrderDate = DateTime.Now;
                order.IsPaid = false;
                order.Status = OrderStatus.Pending;

                // Вызываем синхронный метод
                _orderRepository.AddOrder(order);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("OrderService.CreateOrder", ex.Message);
                throw;
            }
        }
    }
}