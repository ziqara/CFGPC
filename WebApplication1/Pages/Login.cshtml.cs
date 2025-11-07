using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService userService_;
        private readonly SessionManager sessionManager_;

        public LoginModel(UserService userService, SessionManager sessionManager)
        {
            userService_ = userService;
            sessionManager_ = sessionManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }

        public IActionResult OnGet()
        {
            if (sessionManager_.IsUserAuthenticated())
            {
                IsAuthenticated = true;
                UserName = sessionManager_.GetUserNameFromSession();
                return Page();
            }

            IsAuthenticated = false;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (sessionManager_.IsUserAuthenticated())
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string result = userService_.LoginUser(Email, Password);

            if (string.IsNullOrEmpty(result))
            {
                return RedirectToPage("/Index");
            }
            else
            {
                Message = result;
                IsAuthenticated = false;
                return Page();
            }
        }
    }
}