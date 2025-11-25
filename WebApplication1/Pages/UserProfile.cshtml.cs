using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using DDMLib.Configuration; 
using System.ComponentModel.DataAnnotations;

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


        public IActionResult OnGet()
        {
            ErrorLogger.LogError("UserProfileModel OnGet", "Started");
            if (!sessionManager_.IsUserAuthenticated())
            {
                ErrorLogger.LogError("UserProfileModel OnGet", "User not authenticated, redirecting.");
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorLogger.LogError("UserProfileModel OnGet", "User email not found in session, redirecting.");
                return RedirectToPage("/Login");
            }

            LoadUserProfile(userEmail);
            LoadUserConfigurations(userEmail); 
            ErrorLogger.LogError("UserProfileModel OnGet", "Profile and configurations loaded successfully.");
            return Page();
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

            ModelState.Remove(nameof(CurrentPassword));
            ModelState.Remove(nameof(NewPassword));
            ModelState.Remove(nameof(ConfirmPassword));

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
                ViewData["ShowModal"] = "true";
                return Page();
            }

            string result = accountService_.UpdateProfile(userEmail, FullName, Phone, Address);

            if (result == "Профиль обновлён")
            {
                Message = result;
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", "Profile updated successfully.");
                return RedirectToPage();
            }
            else
            {
                ErrorLogger.LogError("UserProfileModel OnPostUpdateProfile", $"Profile update failed: {result}");
                ModelState.AddModelError(string.Empty, result);
                LoadUserProfile(userEmail);
                LoadUserConfigurations(userEmail);
                ViewData["ShowModal"] = "true";
                return Page();
            }
        }

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
            return Page();
        }

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
    }
}