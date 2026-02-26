using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using DDMLib.Component;
using DDMLib.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Pages
{
    public class CreateConfigurationModel : PageModel
    {
        private readonly SessionManager _sessionManager;
        private readonly ComponentService _componentService;
        private readonly ConfigurationService _configurationService;

        public CreateConfigurationModel(
            SessionManager sessionManager,
            ComponentService componentService,
            ConfigurationService configurationService)
        {
            _sessionManager = sessionManager;
            _componentService = componentService;
            _configurationService = configurationService;
        }

        [BindProperty]
        public string ConfigName { get; set; }

        [BindProperty]
        public string TargetUse { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public bool Rgb { get; set; }

        [BindProperty]
        public string Status { get; set; } = "draft";

        [BindProperty]
        public string ComponentIds { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            // Проверяем авторизацию
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            // Проверяем авторизацию
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            // Валидация
            if (string.IsNullOrWhiteSpace(ConfigName))
            {
                ErrorMessage = "Введите название конфигурации";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(TargetUse))
            {
                ErrorMessage = "Выберите цель использования";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(ComponentIds))
            {
                ErrorMessage = "Выберите хотя бы один компонент";
                return Page();
            }

            try
            {
                // Получаем email текущего пользователя через существующий метод
                string userEmail = _sessionManager.GetUserEmailFromSession();

                if (string.IsNullOrEmpty(userEmail))
                {
                    ErrorMessage = "Не удалось определить пользователя";
                    return Page();
                }

                // Парсим ID компонентов
                var componentIdList = ComponentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id)).ToList();

                // Проверка статуса validated - должны быть выбраны все компоненты
                bool allComponentsSelected = AreAllComponentsSelected(componentIdList);

                // Логируем для отладки
                Console.WriteLine($"Статус из формы: {Status}");
                Console.WriteLine($"Все компоненты выбраны: {allComponentsSelected}");

                // Если статус validated, но не все компоненты выбраны - ошибка
                if (Status == "validated" && !allComponentsSelected)
                {
                    ErrorMessage = "Для сохранения как готовой сборки необходимо выбрать все компоненты";
                    return Page();
                }

                // Вычисляем общую стоимость
                decimal totalPrice = CalculateTotalPrice(componentIdList);

                // Создаем объект конфигурации
                var configuration = new DDMLib.Configuration.Configuration
                {
                    ConfigName = ConfigName,
                    Description = Description ?? "",
                    TotalPrice = totalPrice,
                    TargetUse = TargetUse,
                    Status = Status, // Здесь Status приходит из формы
                    IsPreset = false,
                    CreatedDate = DateTime.Now,
                    UserEmail = userEmail,
                    Rgb = Rgb,
                    OtherOptions = ""
                };

                Console.WriteLine($"Сохраняем конфигурацию со статусом: {configuration.Status}");

                // Сохраняем конфигурацию через сервис
                int configId = _configurationService.CreateConfiguration(configuration, componentIdList);

                if (configId > 0)
                {
                    SuccessMessage = "Конфигурация успешно сохранена!";
                    TempData["ClearLocalStorage"] = true;
                    return Page();
                }
                else
                {
                    ErrorMessage = "Ошибка при сохранении конфигурации";
                    return Page();
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении: {ex.Message}";
                return Page();
            }
        }

        private bool AreAllComponentsSelected(List<int> componentIds)
        {
            // Список обязательных типов компонентов
            var requiredTypes = new[] { "cpu", "motherboard", "ram", "gpu", "storage", "psu", "case", "cooling" };

            // Получаем типы выбранных компонентов
            var selectedTypes = new HashSet<string>();

            foreach (var id in componentIds)
            {
                var component = _componentService.GetComponentById(id);
                if (component != null)
                {
                    selectedTypes.Add(component.Type.ToLower());
                }
            }

            // Проверяем, что все обязательные типы присутствуют
            return requiredTypes.All(type => selectedTypes.Contains(type));
        }

        private decimal CalculateTotalPrice(List<int> componentIds)
        {
            decimal total = 0;

            foreach (var id in componentIds)
            {
                var component = _componentService.GetComponentById(id);
                if (component != null)
                {
                    total += component.Price;
                }
            }

            return total;
        }
    }
}