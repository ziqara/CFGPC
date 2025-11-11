using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserService userService_;
        private readonly SessionManager sessionManager_;

        public RegisterModel(UserService userService, SessionManager sessionManager)
        {
            userService_ = userService;
            sessionManager_ = sessionManager;
        }

        [BindProperty]
        public UserValidation NewUser { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 20 символов")]
        public string Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

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

            if (NewUser == null)
                NewUser = new UserValidation();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (sessionManager_.IsUserAuthenticated())
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
                return Page();

            var result = userService_.RegisterUser(
                new User
                {
                    Email = NewUser.Email,
                    FullName = NewUser.FullName,
                    Phone = NewUser.Phone,
                    Address = NewUser.Address,
                    Password = Password
                },
                ConfirmPassword
            );

            if (result == "Аккаунт успешно создан!")
            {
                Message = result;
                NewUser = new UserValidation();
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                ModelState.Clear();
                return Page();
            }

            if (result == "Email уже зарегистрирован")
            {
                ModelState.AddModelError("NewUser.Email", result);
            }
            else
            {
                ModelState.AddModelError(string.Empty, result);
            }

            return Page();
        }
    }
}