using System.ComponentModel;

namespace DDMLib
{
    public class Gpu
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

        // gpus
        [DisplayName("PCIe")]
        public string PcieVersion { get; set; }  // 3.0 / 4.0

        [DisplayName("TDP (Вт)")]
        public int? Tdp { get; set; }

        [DisplayName("VRAM (GB)")]
        public int? VramGb { get; set; }
    }
}