using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.ConfigForAdmin
{
    public class ComponentAdmin
    {
        public int ComponentId { get; set; }
        public string Name { get; set; }  // Название компонента
        public string Description { get; set; }  // Описание компонента
        public decimal Price { get; set; }  // Цена компонента
        public string PhotoUrl { get; set; }  // Ссылка на фото компонента
    }
}
