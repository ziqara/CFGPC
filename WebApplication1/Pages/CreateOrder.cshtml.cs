using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using DDMLib.Order;
using DDMLib.Configuration;
using System.ComponentModel.DataAnnotations;

namespace ClientWebApp.Pages
{
    public class CreateOrderModel : PageModel
    {
        private readonly SessionManager _sessionManager;
        private readonly ConfigurationService _configurationService;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public CreateOrderModel(
            SessionManager sessionManager,
            ConfigurationService configurationService,
            OrderService orderService,
            UserService userService)
        {
            _sessionManager = sessionManager;
            _configurationService = configurationService;
            _orderService = orderService;
            _userService = userService;
        }

        [BindProperty]
        public OrderInputModel OrderInput { get; set; } = new();

        public ConfigurationDto? SelectedConfiguration { get; set; }
        public string? ErrorMessage { get; set; }

        // Свойство для расчета итоговой суммы с доставкой
        public decimal TotalWithDelivery => OrderInput.TotalPrice + (OrderInput.DeliveryMethod == DeliveryMethod.Courier ? 500 : 0);
        public string? UserProfileAddress { get; set; }

        public IActionResult OnGet(int configId)
        {
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var userEmail = _sessionManager.GetUserEmailFromSession();

            // Загружаем конфигурацию - сначала проверяем пользовательские, потом предустановленные
            SelectedConfiguration = LoadConfigurationById(configId, userEmail);

            if (SelectedConfiguration == null)
            {
                ErrorMessage = "Конфигурация не найдена или недоступна для заказа.";
                return Page();
            }

            // Проверяем статус конфигурации (только для пользовательских сборок)
            if (!SelectedConfiguration.Configuration.IsPreset) // Если это не предустановленная сборка
            {
                if (SelectedConfiguration.Configuration.Status != "validated" &&
                    SelectedConfiguration.Configuration.Status != "in_cart")
                {
                    ErrorMessage = "Данная конфигурация недоступна для заказа.";
                    return Page();
                }
            }

            // Загружаем адрес пользователя
            var user = _userService.ValidateUserExists(userEmail);
            if (user != null)
            {
                UserProfileAddress = user.Address;
            }

            // Заполняем данные заказа
            OrderInput.ConfigId = SelectedConfiguration.Configuration.ConfigId;
            OrderInput.TotalPrice = SelectedConfiguration.Configuration.TotalPrice;
            OrderInput.DeliveryMethod = DeliveryMethod.Courier;

            // Автоподстановка только если адрес пустой
            if (string.IsNullOrWhiteSpace(OrderInput.DeliveryAddress) && !string.IsNullOrWhiteSpace(UserProfileAddress))
            {
                OrderInput.DeliveryAddress = UserProfileAddress;
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var userEmail = _sessionManager.GetUserEmailFromSession();

            if (!ModelState.IsValid)
            {
                LoadSelectedConfiguration(userEmail, OrderInput.ConfigId);
                return Page();
            }

            try
            {
                var order = new Order
                {
                    ConfigId = OrderInput.ConfigId,
                    UserEmail = userEmail,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    TotalPrice = TotalWithDelivery,
                    DeliveryAddress = OrderInput.DeliveryMethod == DeliveryMethod.Courier ? OrderInput.DeliveryAddress : null,
                    DeliveryMethod = OrderInput.DeliveryMethod,
                    PaymentMethod = OrderInput.PaymentMethod,
                    IsPaid = false
                };

                _orderService.CreateOrder(order);

                TempData["SuccessMessage"] = "Заказ успешно оформлен!";
                return RedirectToPage("/Orders");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("CreateOrder OnPost", ex.Message);
                ModelState.AddModelError(string.Empty, "Произошла ошибка при оформлении заказа. Пожалуйста, попробуйте снова.");
                LoadSelectedConfiguration(userEmail, OrderInput.ConfigId);
                return Page();
            }
        }

        private ConfigurationDto? LoadConfigurationById(int configId, string userEmail)
        {
            // 1. Сначала ищем в пользовательских конфигурациях
            var userConfigs = _configurationService.GetUserConfigurations(userEmail);
            var userConfig = userConfigs.FirstOrDefault(c => c.Configuration.ConfigId == configId);

            if (userConfig != null)
            {
                return userConfig;
            }

            // 2. Если не нашли, ищем среди предустановленных (isPreset = 1)
            var presetConfigs = _configurationService.GetPresetConfigurations();
            var presetConfig = presetConfigs.FirstOrDefault(c => c.Configuration.ConfigId == configId);

            return presetConfig;
        }

        private void LoadSelectedConfiguration(string userEmail, int configId)
        {
            if (configId > 0)
            {
                SelectedConfiguration = LoadConfigurationById(configId, userEmail);
            }
        }
    }

    public class OrderInputModel : IValidatableObject
    {
        public int ConfigId { get; set; }
        public decimal TotalPrice { get; set; }

        [Display(Name = "Адрес доставки")]
        public string? DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите способ доставки")]
        [Display(Name = "Способ доставки")]
        public DeliveryMethod DeliveryMethod { get; set; }

        [Required(ErrorMessage = "Выберите способ оплаты")]
        [Display(Name = "Способ оплаты")]
        public PaymentMethod PaymentMethod { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DeliveryMethod == DeliveryMethod.Courier && string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                yield return new ValidationResult(
                    "Укажите адрес доставки",
                    new[] { nameof(DeliveryAddress) }
                );
            }
        }
    }
}