using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly SessionManager sessionManager_;
        private readonly AccountService accountService_;
        private readonly UserService userService_;

        public UserProfileModel(SessionManager sessionManager, AccountService accountService, UserService userService)
        {
            sessionManager_ = sessionManager;
            accountService_ = accountService;
            userService_ = userService;
        }

        public User UserProfile { get; set; }
        public string Message { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "ФИО обязательно")]
        [StringLength(255, ErrorMessage = "ФИО не должно превышать 255 символов")]
        public string FullName { get; set; }

        [BindProperty]
        [StringLength(11, ErrorMessage = "Телефон не должен превышать 11 символов")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Телефон должен содержать только цифры")]
        public string? Phone { get; set; }

        [BindProperty]
        [StringLength(500, ErrorMessage = "Адрес не должен превышать 500 символов")]
        public string? Address { get; set; }

        public IActionResult OnGet()
        {
            if (!sessionManager_.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Login");
            }

            LoadUserProfile(userEmail);
            return Page();
        }

        public IActionResult OnPostUpdateProfile()
        {
            if (!sessionManager_.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            string userEmail = sessionManager_.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Login");
            }

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
                LoadUserProfile(userEmail);
                ViewData["ShowModal"] = "true";
                return Page();
            }

            string result = accountService_.UpdateProfile(userEmail, FullName, Phone, Address);

            if (result == "Профиль обновлён")
            {
                Message = result;
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, result);
                LoadUserProfile(userEmail);
                ViewData["ShowModal"] = "true";
                return Page();
            }
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
    }
}