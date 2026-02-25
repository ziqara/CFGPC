using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using DDMLib.Component;

namespace WebApplication1.Pages
{
    public class CreateConfigurationModel : PageModel
    {
        private readonly SessionManager _sessionManager;
        private readonly ComponentService _componentService;

        public CreateConfigurationModel(
            SessionManager sessionManager,
            ComponentService componentService)
        {
            _sessionManager = sessionManager;
            _componentService = componentService;
        }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        // Для будущих свойств

        public IActionResult OnGet()
        {
            // Проверяем авторизацию
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }
    }
}