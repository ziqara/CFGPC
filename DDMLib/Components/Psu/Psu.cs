using System.ComponentModel;

namespace DDMLib
{
    public class Psu
    {
        // components
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

        // psus
        [DisplayName("Мощность (W)")]
        public int? Wattage { get; set; }

        [DisplayName("Сертификация")]
        public string Efficiency { get; set; } // 80+ Bronze / Gold / Platinum
    }
}