using System.ComponentModel;

namespace DDMLib
{
    public class Cooling
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

        [DisplayName("Фото URL")]
        public string PhotoUrl { get; set; }

        [DisplayName("ИНН поставщика")]
        public int? SupplierInn { get; set; }

        // coolings
        [DisplayName("Тип кулера")]
        public string CoolerType { get; set; } // air / liquid (NOT NULL)

        [DisplayName("Поддержка TDP (W)")]
        public int? TdpSupport { get; set; } // nullable

        [DisplayName("Обороты (RPM)")]
        public int? FanRpm { get; set; } // nullable

        [DisplayName("Размер")]
        public string Size { get; set; } // full_tower/mid_tower/compact nullable

        [DisplayName("RGB")]
        public bool IsRgb { get; set; } // tinyint(1)
    }
}