using System.ComponentModel;

namespace DDMLib.Configurations
{
    public class ConfigComponentInfo
    {
        [DisplayName("Тип")]
        public string ComponentType { get; set; }   // CPU/GPU/... (на русском)

        [DisplayName("Компонент")]
        public string ComponentName { get; set; }   // name

        [DisplayName("Бренд")]
        public string Brand { get; set; }

        [DisplayName("Модель")]
        public string Model { get; set; }

        [DisplayName("Кол-во")]
        public int Quantity { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [DisplayName("Поставщик")]
        public string SupplierName { get; set; }
    }
}