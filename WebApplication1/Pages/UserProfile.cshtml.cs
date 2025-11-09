using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;

namespace WebApplication1.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly SessionManager _sessionManager;
        private readonly AccountService _accountService;

        public UserProfileModel(SessionManager sessionManager, AccountService accountService)
        {
            _sessionManager = sessionManager;
            _accountService = accountService;
        }

        public User UserProfile { get; set; }

        public IActionResult OnGet()
        {
            if (!_sessionManager.IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var userEmail = _sessionManager.GetUserEmailFromSession();
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Login");
            }

            UserProfile = _accountService.GetUserProfile(userEmail);

            return Page();
        }
    }
}