using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public class Supplier
    {
        [DisplayName("ИНН")]
        public int Inn { get; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Email")]
        public string ContactEmail { get; set; }

        [DisplayName("Телефон")]
        public string Phone { get; set; }  // может быть null

        [DisplayName("Адрес")]
        public string Address { get; set; } // может быть null

        public Supplier(int inn)
        {
            Inn = inn;
        }
    }

}
