using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using DDMLib.Component;
using DDMLib;

namespace ClientWebApp.Pages
{
    public class ComponentsPageModel : PageModel
    {
        private readonly ComponentService componentService_;

        public ComponentsPageModel(ComponentService componentService)
        {
            componentService_ = componentService;
        }

        public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();

        public void OnGet([FromQuery] string category = "cpu")
        {
            try
            {
                Components = componentService_.GetComponentsByCategory(category);
                ErrorLogger.LogError("OnGet ComponentsPageModel", $"Загружено компонентов: {Components.Count}");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("OnGet ComponentsPageModel", ex.Message);
                Components = new List<ComponentDto>();
            }
        }
    }
}
