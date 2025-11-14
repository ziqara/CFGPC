using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class Component
    {
        public int ComponentId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string PhotoUrl { get; set; }
        public int SupplierId { get; set; }
    }
}
