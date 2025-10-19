using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DDMLib;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        [Required(ErrorMessage = "������� email")]
        [EmailAddress(ErrorMessage = "������������ email")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ������")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Message { get; set; }

        public void OnGet() 
        { 

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string result = _userService.LoginUser(Email, Password);

            if (string.IsNullOrEmpty(result))
            {

                return RedirectToPage("/Index");
            }
            else
            {
                Message = result;  
                return Page();
            }
        }
    }
}