using System.ComponentModel;

namespace DDMLib
{
    public class ComponentItem
    {
        [DisplayName("ID")]
        public int ComponentId { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [DisplayName("Доступен")]
        public bool IsAvailable { get; set; }

        [DisplayName("Остаток")]
        public int StockQuantity { get; set; }

        public override string ToString()
        {
            string avail = (IsAvailable && StockQuantity > 0) ? "" : "  [НЕТ]";
            return $"{Name} ({Price:N2}){avail}";
        }
    }
}