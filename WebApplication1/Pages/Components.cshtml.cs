using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using DDMLib.Component;
using DDMLib.Compatibility;
using System.Text.Json;
using DDMLib;

namespace WebApplication1.Pages
{
    public class ComponentsModel : PageModel
    {
        private readonly ComponentService _componentService;
        private readonly CompatibilityChecker _compatibilityChecker;

        public ComponentsModel(
            ComponentService componentService,
            CompatibilityChecker compatibilityChecker)
        {
            _componentService = componentService;
            _compatibilityChecker = compatibilityChecker;
        }

        public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();
        public Dictionary<string, int> SelectedComponents { get; set; } = new Dictionary<string, int>();

        public IActionResult OnGet([FromQuery] string category = "cpu")
        {
            try
            {
                // Загружаем выбранные компоненты из параметра
                string componentsJson = Request.Query["components"].FirstOrDefault();
                ErrorLogger.LogError("OnGet ComponentsModel", $"componentsJson: {componentsJson ?? "null"}");

                if (!string.IsNullOrEmpty(componentsJson))
                {
                    try
                    {
                        SelectedComponents = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(componentsJson);
                        ErrorLogger.LogError("OnGet ComponentsModel", $"SelectedComponents count: {SelectedComponents?.Count ?? 0}");
                        if (SelectedComponents != null)
                        {
                            foreach (var kv in SelectedComponents)
                            {
                                ErrorLogger.LogError("OnGet ComponentsModel", $"Selected: {kv.Key} = {kv.Value}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError("OnGet ComponentsModel", $"Deserialize error: {ex.Message}");
                        SelectedComponents = new Dictionary<string, int>();
                    }
                }

                // Получаем все компоненты категории
                var allComponents = _componentService.GetComponentsByCategory(category);
                ErrorLogger.LogError("OnGet ComponentsModel", $"allComponents count: {allComponents.Count}");

                // Фильтруем компоненты по совместимости
                if (SelectedComponents != null && SelectedComponents.Any())
                {
                    Components = _compatibilityChecker.FilterCompatibleComponents(category, allComponents, SelectedComponents);
                    ErrorLogger.LogError("OnGet ComponentsModel", $"Filtered components count: {Components.Count}");
                }
                else
                {
                    Components = allComponents;
                    ErrorLogger.LogError("OnGet ComponentsModel", "No selected components, showing all");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("OnGet ComponentsModel", ex.Message);
                Components = new List<ComponentDto>();
            }

            return Page();
        }

        public bool IsComponentCompatible(int componentId)
        {
            if (SelectedComponents == null || !SelectedComponents.Any())
                return true;

            var issues = _compatibilityChecker.CheckCompatibilityForComponent(componentId, SelectedComponents);
            return !issues.Any();
        }

        public List<string> GetCompatibilityIssues(int componentId)
        {
            if (SelectedComponents == null || !SelectedComponents.Any())
                return new List<string>();

            return _compatibilityChecker.CheckCompatibilityForComponent(componentId, SelectedComponents);
        }
    }
}