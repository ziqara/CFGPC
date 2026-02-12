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
            var configurations = _configurationService.GetUserConfigurations(userEmail);

            // Создаем словарь для быстрого поиска конфигурации по ID
            var configDict = configurations.ToDictionary(
                c => c.Configuration.ConfigId,
                c => c.Configuration.ConfigName);

            // Формируем ViewModel с названиями конфигураций
            UserOrders = orders.Select(order => new OrderViewModel
            {
                Order = order,
                ConfigName = configDict.ContainsKey(order.ConfigId)
                    ? configDict[order.ConfigId]
                    : $"Конфигурация #{order.ConfigId}"
            }).ToList();

            return Page();
        }
    }

    public class OrderViewModel
    {
        public Order Order { get; set; }
        public string ConfigName { get; set; }
    }
}