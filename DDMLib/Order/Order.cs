using System;
using System.ComponentModel;

namespace DDMLib.Order
{
    public class Order
    {
        [DisplayName("№ заказа")]
        public int OrderId { get; set; }

        [DisplayName("Конфигурация")]
        public string ConfigName { get; set; }

        [DisplayName("ID конфигурации")]
        public int ConfigId { get; set; }

        [DisplayName("Email пользователя")]
        public string UserEmail { get; set; }

        [DisplayName("Дата заказа")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Статус")]
        public OrderStatus Status { get; set; }

        [DisplayName("Сумма")]
        public decimal TotalPrice { get; set; }

        [DisplayName("Адрес доставки")]
        public string DeliveryAddress { get; set; }

        [DisplayName("Способ доставки")]
        public DeliveryMethod DeliveryMethod { get; set; }

        [DisplayName("Способ оплаты")]
        public PaymentMethod PaymentMethod { get; set; }

        [DisplayName("Оплачен")]
        public bool IsPaid { get; set; }
    }
}