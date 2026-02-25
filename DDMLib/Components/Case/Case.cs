using System.ComponentModel;

namespace DDMLib
{
    public class Case
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

        // cases
        [DisplayName("Форм-фактор")]
        public string FormFactor { get; set; } // varchar(20), nullable

        [DisplayName("Размер корпуса")]
        public string Size { get; set; } // full_tower / mid_tower / compact (NOT NULL)
    }
}