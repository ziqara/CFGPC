using DDMLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare(nameof(NewUser.Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }  // Это поле не в User, но требуется UserService
        public string Message { get; set; }  // Для отображения сообщений
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
            // Вызываем RegisterUser, передавая свойства из User
            string result = _userService.RegisterUser(
                NewUser.Email,
                NewUser.Password,  
                ConfirmPassword,   
                NewUser.FullName,
                NewUser.Phone,
                NewUser.Address
            );
            Message = result;  // Сохраняем результат
            if (result == "Аккаунт успешно создан!")
            {
                return RedirectToPage("/Index");
            }
            return Page(); 
        }
    }
}
