using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using DDMLib.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication1.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly SessionManager sessionManager_;
        private readonly AccountService accountService_;
        private readonly ConfigurationService configurationService_;

        public UserProfileModel(SessionManager sessionManager, AccountService accountService, ConfigurationService configurationService)
        {
            sessionManager_ = sessionManager;
            accountService_ = accountService;
            configurationService_ = configurationService;
        }

        public User UserProfile { get; set; }
        public string Message { get; set; }
        public string PasswordMessage { get; set; }
        public List<ConfigurationDto> UserConfigurations { get; set; } = new List<ConfigurationDto>();

        // --- Новые свойства для аватара ---
        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        public string? AvatarBase64 { get; set; }
        public string? AvatarMimeType { get; set; } // Добавлено для определения MIME типа
        // -------------------------------------

        [BindProperty]
        [Required(ErrorMessage = "ФИО обязательно", AllowEmptyStrings = false)]
        [StringLength(255, ErrorMessage = "ФИО не должно превышать 255 символов")]
        public string FullName { get; set; }

        [BindProperty]
        [StringLength(11, ErrorMessage = "Телефон не должен превышать 11 символов")]
        [RegularExpression(@"^(\d{11,11})$", ErrorMessage = "Телефон должен содержать только цифры и иметь 11 символов.")]
        public string? Phone { get; set; }

        [BindProperty]
        [StringLength(500, ErrorMessage = "Адрес не должен превышать 500 символов")]
        public string? Address { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите текущий пароль")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите новый пароль")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 20 символов")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
            ErrorLogger.LogError("UserProfileModel OnGet", "Started");
            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnGet", "User not authenticated, redirecting.");
                RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorLogger.LogError("UserProfileModel OnGet", "User email not found in session, redirecting.");
                RedirectToPage("/Login");
            }

            LoadUserProfile(userEmail);
            LoadUserConfigurations(userEmail);

            if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
            {
                AvatarMimeType = GetMimeType(UserProfile.Avatar);
                AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
            }

            ErrorLogger.LogError("UserProfileModel OnGet", "Profile and configurations loaded successfully.");
        }

        public IActionResult OnPostUpdateProfile()
        {
            ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", "Started");
            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", "User not authenticated, redirecting.");
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", "User email not found in session, redirecting.");
                return RedirectToPage("/Login");
            }

            // Удаляем проверки пароля из модели состояния при обновлении профиля
            ModelState.Remove(nameof(CurrentPassword));
            ModelState.Remove(nameof(NewPassword));
            ModelState.Remove(nameof(ConfirmPassword));

            // Удаляем проверки телефон/адрес, если они пустые
            if (string.IsNullOrEmpty(Phone))
            {
                ModelState.Remove("Phone");
            }
            if (string.IsNullOrEmpty(Address))
            {
                ModelState.Remove("Address");
            }

            if (!ModelState.IsValid)
            {
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", "ModelState is invalid.");
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarMimeType = GetMimeType(UserProfile.Avatar);
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                ViewData["ShowModal"] = "true";
                return Page();
            }

            string result = accountService_.UpdateProfile(userEmail, FullName, Phone, Address);

            if (result == "Профиль обновлён")
            {
                Message = result;
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", "Profile updated successfully.");
                // Перезагружаем данные после обновления
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarMimeType = GetMimeType(UserProfile.Avatar);
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                return Page(); // Остаемся на странице, чтобы увидеть сообщение
            }
            else
            {
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", $"Profile update failed: {result}");
                ModelState.AddModelError(string.Empty, result);
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarMimeType = GetMimeType(UserProfile.Avatar);
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                ViewData["ShowModal"] = "true";
                return Page();
            }
        }

        // --- Новый обработчик для загрузки аватара ---
        public async Task<IActionResult> OnPostUploadAvatarAsync()
        {
            ErrorLogger.LogError("UserProfileModel OnPostUploadAvatarAsync", "Started");
            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnPostUploadAvatarAsync", "User not authenticated, redirecting.");
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorLogger.LogError("UserProfileModel OnPostUploadAvatarAsync", "User email not found in session, redirecting.");
                return RedirectToPage("/Login");
            }

            if (AvatarFile == null || AvatarFile.Length == 0)
            {
                Message = "Файл аватара не выбран.";
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                return Page();
            }

            // Проверка типа файла (опционально, но рекомендуется)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                Message = "Недопустимый формат файла. Разрешены: JPG, JPEG, PNG, GIF.";
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                return Page();
            }

            // Проверка размера файла (например, не более 5 МБ)
            if (AvatarFile.Length > 5 * 1024 * 1024)
            {
                Message = "Файл слишком большой. Максимальный размер 5 МБ.";
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                return Page();
            }

            byte[] avatarBytes;
            using (var memoryStream = new MemoryStream())
            {
                await AvatarFile.CopyToAsync(memoryStream);
                avatarBytes = memoryStream.ToArray();
            }

            // Вызов сервиса для обновления аватара
            string result = accountService_.UpdateUserAvatar(userEmail, avatarBytes);

            if (result == "Аватар обновлён")
            {
                Message = result;
                ErrorLogger.LogError("UserProfileModel OnPostUploadAvatarAsync", "Avatar updated successfully.");
            }
            else
            {
                ErrorLogger.LogError("UserProfileModel OnPostUploadAvatarAsync", $"Avatar update failed: {result}");
                Message = result; // Отображаем сообщение об ошибке от сервиса
            }

            // Перезагружаем данные после обновления аватара
            LoadUserProfile(userEmail);
            LoadUserConfigurations(userEmail);
            // --- Установка Base64 для аватара ---
            if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
            {
                AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
            }
            // -------------------------------------
            return Page();
        }
    // -------------------------------------------

        public IActionResult OnPostChangePassword()
        {
            ErrorLogger.LogError("UserProfileModel OnPostChangePassword", "Started");
            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnPostChangePassword", "User not authenticated, redirecting.");
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            ErrorLogger.LogError("UserProfileModel OnPostChangePassword", $"Session email retrieved: '{userEmail}'");

            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorLogger.LogError("UserProfileModel OnPostChangePassword", "User email not found in session, redirecting.");
                return RedirectToPage("/Login");
            }

            PasswordMessage = string.Empty;
            Message = string.Empty;

            // Удаляем поля профиля из ModelState при смене пароля
            ModelState.Remove(nameof(FullName));
            ModelState.Remove(nameof(Phone));
            ModelState.Remove(nameof(Address));

            if (!ModelState.IsValid)
            {
                ErrorLogger.LogError("UserProfileModel OnPostChangePassword", "ModelState is invalid after removing profile fields.");
                foreach (var error in ModelState)
                {
                    if (error.Key == nameof(CurrentPassword) || error.Key == nameof(NewPassword) || error.Key == nameof(ConfirmPassword))
                    {
                        foreach (var subError in error.Value.Errors)
                        {
                            ErrorLogger.LogError("UserProfileModel OnPostChangePassword", $"Password Change ModelState Error in '{error.Key}': {subError.ErrorMessage}");
                        }
                    }
                }
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarMimeType = GetMimeType(UserProfile.Avatar);
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                return Page();
            }

            ErrorLogger.LogError("UserProfileModel OnPostChangePassword", "ModelState is valid for password change. Calling AccountService.");

            string result = accountService_.ChangePassword(userEmail, CurrentPassword, NewPassword, ConfirmPassword);
            ErrorLogger.LogError("UserProfileModel OnPostChangePassword", $"AccountService returned: '{result}'");

            if (result == "Пароль обновлён")
            {
                PasswordMessage = result;
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                ErrorLogger.LogError("UserProfileModel OnPostChangePassword", "Password changed successfully.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result);
                ErrorLogger.LogError("UserProfileModel OnPostChangePassword", $"Password change failed: {result}");
            }

            LoadUserProfile(userEmail);
            LoadUserConfigurations(userEmail);
            // --- Установка Base64 для аватара ---
            if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
            {
                AvatarMimeType = GetMimeType(UserProfile.Avatar);
                AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
            }
            // -------------------------------------
            return Page();
        }

        // --- Новый обработчик для удаления конфигурации ---
        public IActionResult OnPostDeleteConfiguration(int configId)
        {
            ErrorLogger.LogError("UserProfileModel OnPostDeleteConfiguration", $"Started for configId: {configId}");
            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnPostDeleteConfiguration", "User not authenticated, redirecting.");
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorLogger.LogError("UserProfileModel OnPostDeleteConfiguration", "User email not found in session, redirecting.");
                return RedirectToPage("/Login");
            }

            if (configId <= 0)
            {
                ErrorLogger.LogError("UserProfileModel OnPostDeleteConfiguration", "Invalid configId provided.");
                Message = "Неверный идентификатор конфигурации.";
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                // --- Установка Base64 для аватара ---
                if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
                {
                    AvatarMimeType = GetMimeType(UserProfile.Avatar);
                    AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
                }
                // -------------------------------------
                return Page();
            }

            string result = configurationService_.DeleteUserConfiguration(userEmail, configId);

            if (string.IsNullOrEmpty(result))
            {
                Message = "Конфигурация удалена.";
                ErrorLogger.LogError("UserProfileModel OnPostDeleteConfiguration", $"Configuration {configId} deleted successfully.");
            }
            else
            {
                ErrorLogger.LogError("UserProfileModel OnPostDeleteConfiguration", $"Failed to delete configuration {configId}: {result}");
                Message = result; // Отображаем сообщение об ошибке от сервиса
            }

            // Перезагружаем данные после удаления
            LoadUserProfile(userEmail);
            LoadUserConfigurations(userEmail);
            // --- Установка Base64 для аватара ---
            if (UserProfile?.Avatar != null && UserProfile.Avatar.Length > 0)
            {
                AvatarMimeType = GetMimeType(UserProfile.Avatar);
                AvatarBase64 = Convert.ToBase64String(UserProfile.Avatar);
            }
            // -------------------------------------
            return Page(); // Возвращаемся на ту же страницу, чтобы увидеть обновлённый список и сообщение
        }
        // -------------------------------------------

        public IActionResult OnPostLogout()
        {
            ErrorLogger.LogError("UserProfileModel OnPostLogout", "Logout initiated.");

            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnPostLogout", "User not authenticated, cannot logout.");
            }

            accountService_.Logout();

            ErrorLogger.LogError("UserProfileModel OnPostLogout", "Logout process completed, redirecting to Login.");

            return RedirectToPage("/Login");
        }

        private void LoadUserProfile(string email)
        {
            UserProfile = accountService_.GetUserProfile(email);
            if (UserProfile != null)
            {
                FullName = UserProfile.FullName;
                Phone = UserProfile.Phone;
                Address = UserProfile.Address;
            }
        }

        private void LoadUserConfigurations(string email)
        {
            try
            {
                UserConfigurations = configurationService_.GetUserConfigurations(email);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("UserProfileModel LoadUserConfigurations", ex.Message);
                UserConfigurations = new List<ConfigurationDto>(); // В случае ошибки возвращаем пустой список
            }
        }

        // Метод для определения MIME типа по байтам изображения
        private string GetMimeType(byte[] imageBytes)
        {
            if (imageBytes.Length < 4) return "image/png"; // По умолчанию

            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8) return "image/jpeg";
            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47) return "image/png";
            if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46) return "image/gif";

            return "image/png"; // По умолчанию
        }
    }
}