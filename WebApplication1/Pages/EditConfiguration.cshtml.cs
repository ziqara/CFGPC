using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using DDMLib.Component;
using DDMLib.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace WebApplication1.Pages
{
    public class EditConfigurationModel : PageModel
    {
        private readonly SessionManager _sessionManager;
        private readonly ComponentService _componentService;
        private readonly ConfigurationService _configurationService;

        public EditConfigurationModel(
            SessionManager sessionManager,
            ComponentService componentService,
            ConfigurationService configurationService)
        {
            _sessionManager = sessionManager;
            _componentService = componentService;
            _configurationService = configurationService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public int ConfigId { get; set; }

        [BindProperty]
        public string ConfigName { get; set; }

        [BindProperty]
        public string TargetUse { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public bool Rgb { get; set; }

        [BindProperty]
        public string Status { get; set; }

        [BindProperty]
        public string ComponentIds { get; set; }

        public decimal TotalPrice { get; set; }

        public List<int> ComponentIdList { get; set; } = new List<int>();

        // Для передачи в JavaScript
        public List<ComponentData> Components { get; set; } = new List<ComponentData>();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            // Проверяем авторизацию
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            string userEmail = _sessionManager.GetUserEmailFromSession();

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Login");
            }

            // Загружаем конфигурацию
            var configDto = _configurationService.GetConfigurationById(id);

            if (configDto == null || configDto.Configuration == null)
            {
                ErrorMessage = "Конфигурация не найдена";
                return RedirectToPage("/UserProfile");
            }

            if (configDto.Configuration.UserEmail != userEmail)
            {
                ErrorMessage = "Доступ запрещен";
                return RedirectToPage("/UserProfile");
            }

            // Заполняем поля формы
            ConfigId = configDto.Configuration.ConfigId;
            ConfigName = configDto.Configuration.ConfigName;
            TargetUse = configDto.Configuration.TargetUse;
            Description = configDto.Configuration.Description ?? "";
            Rgb = configDto.Configuration.Rgb;
            Status = configDto.Configuration.Status;
            TotalPrice = configDto.Configuration.TotalPrice;

            // Заполняем список ID компонентов
            ComponentIdList = configDto.Components?.Select(c => c.ComponentId).ToList() ?? new List<int>();
            ComponentIds = string.Join(",", ComponentIdList);

            // Заполняем данные компонентов для JavaScript
            if (configDto.Components != null)
            {
                foreach (var comp in configDto.Components)
                {
                    try
                    {
                        var spec = _componentService.GetComponentSpec<object>(comp.ComponentId);
                        Components.Add(new ComponentData
                        {
                            Id = comp.ComponentId,
                            Name = comp.Name,
                            Type = comp.Type?.ToLower() ?? "",
                            Price = comp.Price,
                            Specs = spec ?? new object()
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError("EditConfigurationModel.OnGet", $"Error loading spec for component {comp.ComponentId}: {ex.Message}");
                        // Добавляем компонент без спецификаций
                        Components.Add(new ComponentData
                        {
                            Id = comp.ComponentId,
                            Name = comp.Name,
                            Type = comp.Type?.ToLower() ?? "",
                            Price = comp.Price,
                            Specs = new { }
                        });
                    }
                }
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

            string userEmail = _sessionManager.GetUserEmailFromSession();

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
                var componentIdList = ComponentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id)).ToList();

                // Проверка статуса validated
                if (Status == "validated" && !AreAllComponentsSelected(componentIdList))
                {
                    ErrorMessage = "Для сохранения как готовой сборки необходимо выбрать все компоненты";
                    return Page();
                }

                decimal totalPrice = CalculateTotalPrice(componentIdList);

                var configuration = new DDMLib.Configuration.Configuration
                {
                    ConfigId = ConfigId,
                    ConfigName = ConfigName,
                    Description = Description ?? "",
                    TotalPrice = totalPrice,
                    TargetUse = TargetUse,
                    Status = Status,
                    IsPreset = false,
                    UserEmail = userEmail,
                    Rgb = Rgb,
                    OtherOptions = ""
                };

                // Обновляем конфигурацию
                bool updated = _configurationService.UpdateConfiguration(configuration, componentIdList);

                if (updated)
                {
                    SuccessMessage = "Конфигурация успешно обновлена!";
                    TempData["ClearLocalStorage"] = true;
                    return RedirectToPage("/UserProfile");
                }
                else
                {
                    ErrorMessage = "Ошибка при обновлении конфигурации";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                return Page();
            }
        }

        private bool AreAllComponentsSelected(List<int> componentIds)
        {
            var requiredTypes = new[] { "cpu", "motherboard", "ram", "gpu", "storage", "psu", "case", "cooling" };
            var selectedTypes = new HashSet<string>();

            foreach (var id in componentIds)
            {
                var component = _componentService.GetComponentById(id);
                if (component != null)
                {
                    selectedTypes.Add(component.Type.ToLower());
                }
            }

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

    public class ComponentData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public object Specs { get; set; }
    }
}