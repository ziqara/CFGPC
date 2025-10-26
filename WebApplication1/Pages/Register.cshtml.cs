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
        public User NewUser { get; set; }

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
            NewUser = new User();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(NewUser.FullName))
            {
                ModelState.AddModelError("NewUser.FullName", "Полное имя обязательно");
            }

            if (!string.IsNullOrEmpty(NewUser.Phone))
            {
                var phoneAttr = new PhoneAttribute();
                if (!phoneAttr.IsValid(NewUser.Phone))
                {
                    ModelState.AddModelError("NewUser.Phone", phoneAttr.ErrorMessage);
                }
            }


            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            NewUser.Password = Password;

            string result = _userService.RegisterUser(
                NewUser, ConfirmPassword
            );

            Message = result;

            if (result == "Аккаунт успешно создан!")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}