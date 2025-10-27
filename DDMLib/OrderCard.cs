using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class OrderCard
    {
        public int OrderId { get; set; }
        public int ConfigId { get; set; }
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
    }
}
