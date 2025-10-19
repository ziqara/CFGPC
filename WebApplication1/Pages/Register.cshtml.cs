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
        public User NewUser { get; set; }  // Используем класс User

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            NewUser = new User();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            // Ручная проверка паролей
            if (string.IsNullOrEmpty(ConfirmPassword) || NewUser.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Пароли не совпадают");
                return Page();  // Вернуться с ошибкой
            }

            
            string result = _userService.RegisterUser(NewUser, ConfirmPassword);

            Message = result;

            if (result == "Аккаунт успешно создан!")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}