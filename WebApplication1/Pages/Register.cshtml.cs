using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserService _userService;

        public RegisterModel(UserService userService)
        {
            _userService = userService;
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

        public void OnGet()
        {
            if (NewUser == null)
                NewUser = new UserValidation();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = _userService.RegisterUser(
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
