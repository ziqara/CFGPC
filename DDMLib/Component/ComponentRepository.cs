using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    public class ComponentRepository : IComponentRepository
    {
        public List<ComponentDto> GetComponentsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Категория не может быть пустой.", nameof(category));
            }

            HashSet<string> validCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "cpu", "motherboard", "ram", "gpu", "storage", "psu", "case", "cooling"
            };

            if (!validCategories.Contains(category))
            {
                throw new ArgumentException("Недопустимое значение категории.", nameof(category));
            }

        }
    }
}
