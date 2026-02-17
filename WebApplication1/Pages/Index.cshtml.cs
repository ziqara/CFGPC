using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib.Configuration;
using DDMLib;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ConfigurationService _configurationService;

        public IndexModel(ILogger<IndexModel> logger, ConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
        }

        [BindProperty(SupportsGet = true)]
        public string FilterTargetUse { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public int ShowCount { get; set; } = 3;

        public List<string> AvailableTargetUses { get; set; } = new();
        public List<ConfigurationDto> AllConfigurations { get; set; } = new();
        public List<ConfigurationDto> DisplayedConfigurations { get; set; } = new();

        public void OnGet()
        {
            // Загружаем все предустановки
            AllConfigurations = _configurationService.GetPresetConfigurations();

            // Получаем уникальные значения targetUse
            AvailableTargetUses = AllConfigurations
                .Select(c => c.Configuration.TargetUse)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            // Фильтруем по targetUse, если указан
            var filtered = AllConfigurations.AsEnumerable();
            if (!string.IsNullOrEmpty(FilterTargetUse))
            {
                filtered = filtered.Where(c => c.Configuration.TargetUse == FilterTargetUse);
            }

            // Сортируем по цене (опционально)
            filtered = filtered.OrderBy(c => c.Configuration.TotalPrice);

            // Берем первые N
            DisplayedConfigurations = filtered.Take(ShowCount).ToList();
        }

        public IActionResult OnPostLoadMore()
        {
            OnGet(); // Загружаем всё снова
            ShowCount += 3; // Увеличиваем количество
            return Partial("_ConfigCards", DisplayedConfigurations);
        }
    }
}