using System;

namespace DDMLib.Order
{
    public static class OrderEnumExtensions
    {
        public static string ToRussian(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending: return "Ожидает";
                case OrderStatus.Processing: return "В обработке";
                case OrderStatus.Assembled: return "Собран";
                case OrderStatus.Shipped: return "Отправлен";
                case OrderStatus.Delivered: return "Доставлен";
                case OrderStatus.Cancelled: return "Отменён";
                default: return status.ToString();
            }
        }

        public static string ToRussian(this PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.Card: return "Карта";
                case PaymentMethod.CashOnDelivery: return "Наложенный платёж";
                case PaymentMethod.BankTransfer: return "Банковский перевод";
                default: return method.ToString();
            }
        }

        public static string ToRussian(this DeliveryMethod method)
        {
            switch (method)
            {
                case DeliveryMethod.Courier: return "Курьер";
                case DeliveryMethod.Pickup: return "Самовывоз";
                case DeliveryMethod.Self: return "Со склада";
                default: return method.ToString();
            }
        }
    }
}