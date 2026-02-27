using System;

namespace DDMLib
{
    public class BuildCard
    {
        public int ConfigId { get; set; }
        public string ConfigName { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsPreset { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserEmail { get; set; }

        public int SalesCount { get; set; }      // сколько раз заказали (продаж)
        public int BadComponents { get; set; }    // сколько недоступных/нет на складе компонентов

        public bool CanDelete => SalesCount == 0;
        public bool HasAvailabilityProblems => BadComponents > 0;
    }
}