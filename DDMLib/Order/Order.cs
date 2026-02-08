using System;

namespace DDMLib.Order
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ConfigId { get; set; }
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
    }
}
