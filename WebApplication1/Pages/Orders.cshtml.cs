// Pages/Orders.cshtml.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib.Order;
using DDMLib;

namespace ClientWebApp.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly OrderService _orderService;
        private readonly SessionManager _sessionManager;

        public List<Order> UserOrders { get; set; } = new List<Order>();

        public OrdersModel(OrderService orderService, SessionManager sessionManager)
        {
            _orderService = orderService;
            _sessionManager = sessionManager;
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
            UserOrders = _orderService.GetOrdersByUserEmail(userEmail);

            return Page();
        }
    }
}