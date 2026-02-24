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
            LoadConfigurations();
        }

        public IActionResult OnGetLoadMore(string filterTargetUse, int showCount)
        {
            FilterTargetUse = filterTargetUse ?? "";
            ShowCount = showCount;

            LoadConfigurations();

            return Partial("_ConfigCards", DisplayedConfigurations);
        }

        private void LoadConfigurations()
        {
            // Загружаем все предустановки (isPreset = 1)
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

            // Сортируем по цене
            filtered = filtered.OrderBy(c => c.Configuration.TotalPrice);

            // Берем первые ShowCount (если ShowCount очень большой, берем все)
            DisplayedConfigurations = filtered.Take(ShowCount).ToList();
        }
    }
}