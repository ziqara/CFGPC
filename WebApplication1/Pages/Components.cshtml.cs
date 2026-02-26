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
                // Получаем все компоненты категории
                var allComponents = _componentService.GetComponentsByCategory(category);

                // Загружаем выбранные компоненты из параметра
                string componentsJson = Request.Query["components"].FirstOrDefault();
                if (!string.IsNullOrEmpty(componentsJson))
                {
                    try
                    {
                        SelectedComponents = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(componentsJson);
                    }
                    catch { }
                }

                // Фильтруем компоненты по совместимости
                // Если есть выбранные компоненты, показываем только совместимые
                if (SelectedComponents != null && SelectedComponents.Any())
                {
                    Components = _compatibilityChecker.FilterCompatibleComponents(category, allComponents, SelectedComponents);
                    ErrorLogger.LogError("OnGet ComponentsModel", $"Отфильтровано компонентов: {Components.Count} из {allComponents.Count}");
                }
                else
                {
                    // Если нет выбранных компонентов, показываем все
                    Components = allComponents;
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