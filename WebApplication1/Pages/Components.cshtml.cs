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

        public void OnGet([FromQuery] string category = "cpu", [FromQuery] string components = null)
        {
            try
            {
                Components = _componentService.GetComponentsByCategory(category);

                // Загружаем выбранные компоненты из параметра
                if (!string.IsNullOrEmpty(components))
                {
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(components);
                        SelectedComponents = dict.ToDictionary(
                            kv => kv.Key,
                            kv => int.Parse(kv.Value.ToString())
                        );
                    }
                    catch { }
                }

                ErrorLogger.LogError("OnGet ComponentsModel", $"Загружено компонентов: {Components.Count}");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("OnGet ComponentsModel", ex.Message);
                Components = new List<ComponentDto>();
            }
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