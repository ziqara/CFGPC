using System;
using System.Collections.Generic;
using DDMLib.ConfigForAdmin;

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

        public int OrdersCount { get; set; }      // сколько заказов на эту сборку
        public int BadComponents { get; set; }    // сколько недоступных/нет на складе компонентов

        public bool CanDelete => OrdersCount == 0;
        public bool HasAvailabilityProblems => BadComponents > 0;

        public string Description { get; set; }  // Добавлено описание сборки
        public string TargetUse { get; set; }  // Цель использования (gaming, professional, office, student)
        public string Status { get; set; }  // Статус сборки
        public string OtherOptions { get; set; }  // Дополнительные параметры

        // Метод для получения компонентов сборки
        public List<ComponentAdmin> GetComponents()
        {
            // Здесь может быть логика получения компонентов для этой сборки
            return new List<ComponentAdmin>(); // Пример возвращаемого значения
        }

    }
}