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
        [Required(ErrorMessage = "������������� ������ �����������")]
        [DataType(DataType.Password)]
        [Compare("NewUser.Password", ErrorMessage = "������ �� ���������")]
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
                ModelState.AddModelError("NewUser.FullName", "������ ��� �����������");
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

            
            if (string.IsNullOrEmpty(ConfirmPassword) || NewUser.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "������ �� ���������");
                return Page();  
            }

            
            string result = _userService.RegisterUser(
                NewUser, ConfirmPassword
            );

            Message = result;

            if (result == "������� ������� ������!")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}