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

        // Для передачи в JavaScript - используем Dictionary для надежности
        public List<ComponentData> Components { get; set; } = new List<ComponentData>();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            if (!_sessionManager.IsUserAuthenticated())
                return RedirectToPage("/Login");

            string userEmail = _sessionManager.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToPage("/Login");

            var configDto = _configurationService.GetConfigurationById(id);
            if (configDto?.Configuration == null)
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

            ComponentIdList = configDto.Components?.Select(c => c.ComponentId).ToList() ?? new List<int>();
            ComponentIds = string.Join(",", ComponentIdList);

            // Загружаем спецификации для каждого компонента
            if (configDto.Components != null)
            {
                foreach (var comp in configDto.Components)
                {
                    try
                    {
                        var spec = GetComponentSpecAsDictionary(comp.ComponentId, comp.Type);
                        Components.Add(new ComponentData
                        {
                            Id = comp.ComponentId,
                            Name = comp.Name,
                            Type = comp.Type?.ToLower() ?? "",
                            Price = comp.Price,
                            Specs = spec ?? new Dictionary<string, object>()
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError("EditConfigurationModel.OnGet", $"Error loading spec for component {comp.ComponentId}: {ex.Message}");
                        Components.Add(new ComponentData
                        {
                            Id = comp.ComponentId,
                            Name = comp.Name,
                            Type = comp.Type?.ToLower() ?? "",
                            Price = comp.Price,
                            Specs = new Dictionary<string, object>()
                        });
                    }
                }
            }

            return Page();
        }

        private Dictionary<string, object>? GetComponentSpecAsDictionary(int componentId, string componentType)
        {
            switch (componentType?.ToLower())
            {
                case "cpu":
                    var cpuSpec = _componentService.GetComponentSpec<CpuSpec>(componentId);
                    return cpuSpec != null ? new Dictionary<string, object>
                    {
                        ["socket"] = cpuSpec.Socket ?? "",
                        ["cores"] = cpuSpec.Cores,
                        ["tdp"] = cpuSpec.Tdp
                    } : null;

                case "motherboard":
                    var mbSpec = _componentService.GetComponentSpec<MotherboardSpec>(componentId);
                    return mbSpec != null ? new Dictionary<string, object>
                    {
                        ["socket"] = mbSpec.Socket ?? "",
                        ["ramType"] = mbSpec.RamType ?? "",
                        ["formFactor"] = mbSpec.FormFactor ?? "",
                        ["chipset"] = mbSpec.Chipset ?? "",
                        ["pcieVersion"] = mbSpec.PcieVersion ?? ""
                    } : null;

                case "ram":
                    var ramSpec = _componentService.GetComponentSpec<RamSpec>(componentId);
                    return ramSpec != null ? new Dictionary<string, object>
                    {
                        ["ramType"] = ramSpec.RamType ?? "",
                        ["capacityGb"] = ramSpec.CapacityGb,
                        ["speedMhz"] = ramSpec.SpeedMhz,
                        ["slotsNeeded"] = ramSpec.SlotsNeeded
                    } : null;

                case "gpu":
                    var gpuSpec = _componentService.GetComponentSpec<GpuSpec>(componentId);
                    return gpuSpec != null ? new Dictionary<string, object>
                    {
                        ["pcieVersion"] = gpuSpec.PcieVersion ?? "",
                        ["tdp"] = gpuSpec.Tdp,
                        ["vramGb"] = gpuSpec.VramGb
                    } : null;

                case "storage":
                    var storageSpec = _componentService.GetComponentSpec<StorageSpec>(componentId);
                    return storageSpec != null ? new Dictionary<string, object>
                    {
                        ["interface"] = storageSpec.Interface ?? "",
                        ["capacityGb"] = storageSpec.CapacityGb
                    } : null;

                case "psu":
                    var psuSpec = _componentService.GetComponentSpec<PsuSpec>(componentId);
                    return psuSpec != null ? new Dictionary<string, object>
                    {
                        ["wattage"] = psuSpec.Wattage,
                        ["efficiencyRating"] = psuSpec.EfficiencyRating ?? ""
                    } : null;

                case "case":
                    var caseSpec = _componentService.GetComponentSpec<CaseSpec>(componentId);
                    return caseSpec != null ? new Dictionary<string, object>
                    {
                        ["formFactor"] = caseSpec.FormFactor ?? "",
                        ["size"] = caseSpec.Size ?? ""
                    } : null;

                case "cooling":
                    var coolingSpec = _componentService.GetComponentSpec<CoolingSpec>(componentId);
                    return coolingSpec != null ? new Dictionary<string, object>
                    {
                        ["coolerType"] = coolingSpec.CoolerType ?? "",
                        ["tdpSupport"] = coolingSpec.TdpSupport,
                        ["fanRpm"] = coolingSpec.FanRpm,
                        ["size"] = coolingSpec.Size ?? "",
                        ["isRgb"] = coolingSpec.IsRgb
                    } : null;

                default:
                    return null;
            }
        }

        public IActionResult OnPost()
        {
            if (!_sessionManager.IsUserAuthenticated())
                return RedirectToPage("/Login");

            string userEmail = _sessionManager.GetUserEmailFromSession();

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
        public Dictionary<string, object> Specs { get; set; }
    }
}