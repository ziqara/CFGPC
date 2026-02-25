// Pages/Orders.cshtml.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib.Order;
using DDMLib;
using DDMLib.Configuration;

namespace ClientWebApp.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly OrderService _orderService;
        private readonly SessionManager _sessionManager;
        private readonly ConfigurationService _configurationService;

        public List<OrderViewModel> UserOrders { get; set; } = new List<OrderViewModel>();

        public OrdersModel(
            OrderService orderService,
            SessionManager sessionManager,
            ConfigurationService configurationService)
        {
            _orderService = orderService;
            _sessionManager = sessionManager;
            _configurationService = configurationService;
        }

        public IActionResult OnGet()
        {
            // Проверяем авторизацию пользователя
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            // Получаем email авторизованного пользователя
            var userEmail = _sessionManager.GetUserEmailFromSession();

            // Получаем заказы пользователя
            var orders = _orderService.GetOrdersByUserEmail(userEmail);

            // Получаем все конфигурации пользователя
            var userConfigurations = _configurationService.GetUserConfigurations(userEmail);

            // Получаем все предустановленные конфигурации
            var presetConfigurations = _configurationService.GetPresetConfigurations();

            // Создаем общий словарь для быстрого поиска конфигурации по ID
            var allConfigurations = new Dictionary<int, ConfigurationDto>();

            // Добавляем пользовательские конфигурации
            foreach (var config in userConfigurations)
            {
                if (!allConfigurations.ContainsKey(config.Configuration.ConfigId))
                {
                    allConfigurations[config.Configuration.ConfigId] = config;
                }
            }

            // Добавляем предустановленные конфигурации
            foreach (var config in presetConfigurations)
            {
                if (!allConfigurations.ContainsKey(config.Configuration.ConfigId))
                {
                    allConfigurations[config.Configuration.ConfigId] = config;
                }
            }

            // Формируем ViewModel с названиями и деталями конфигураций
            UserOrders = orders.Select(order => new OrderViewModel
            {
                Order = order,
                Configuration = allConfigurations.ContainsKey(order.ConfigId)
                    ? allConfigurations[order.ConfigId]
                    : null
            }).ToList();

            return Page();
        }
    }

    public class OrderViewModel
    {
        public Order Order { get; set; }
        public ConfigurationDto Configuration { get; set; }

        public string ConfigName => Configuration?.Configuration?.ConfigName ?? $"Конфигурация #{Order.ConfigId}";
        public bool HasConfiguration => Configuration != null;
    }
}