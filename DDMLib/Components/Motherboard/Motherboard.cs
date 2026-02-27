using System.ComponentModel;

namespace DDMLib
{
    public class Motherboard
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

        // motherboards
        [DisplayName("Socket")]
        public string Socket { get; set; }

        [DisplayName("Chipset")]
        public string Chipset { get; set; }

        [DisplayName("Тип RAM")]
        public string RamType { get; set; } // DDR4 / DDR5

        [DisplayName("PCIe")]
        public string PcieVersion { get; set; } // 3.0 / 4.0 / 5.0

        [DisplayName("Форм-фактор")]
        public string FormFactor { get; set; } // ATX / mATX / ITX и т.п.
    }
}