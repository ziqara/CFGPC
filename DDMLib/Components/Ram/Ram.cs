using System.ComponentModel;

namespace DDMLib
{
    public class Ram
    {
        [DisplayName("ID")]
        public int ComponentId { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Бренд")]
        public string Brand { get; set; }

        [DisplayName("Модель")]
        public string Model { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        [DisplayName("Остаток")]
        public int StockQuantity { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Доступен")]
        public bool IsAvailable { get; set; }

        [DisplayName("Путь к фото")]
        public string PhotoUrl { get; set; }

        [DisplayName("ИНН поставщика")]
        public int? SupplierInn { get; set; }

        // rams
        [DisplayName("Тип RAM")]
        public string RamType { get; set; }   // DDR4/DDR5

        [DisplayName("Объём (GB)")]
        public int? CapacityGb { get; set; }

        [DisplayName("Частота (MHz)")]
        public int? SpeedMhz { get; set; }

        [DisplayName("Слоты")]
        public int? SlotsNeeded { get; set; }
    }
}