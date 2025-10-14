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
        [Required(ErrorMessage = "������������� ������ �����������")]
        [Compare(nameof(NewUser.Password), ErrorMessage = "������ �� ���������")]
        public string ConfirmPassword { get; set; }  // ��� ���� �� � User, �� ��������� UserService
        public string Message { get; set; }  // ��� ����������� ���������
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
            // �������� RegisterUser, ��������� �������� �� User
            string result = _userService.RegisterUser(
                NewUser.Email,
                NewUser.Password,  
                ConfirmPassword,   
                NewUser.FullName,
                NewUser.Phone,
                NewUser.Address
            );
            Message = result;  // ��������� ���������
            if (result == "������� ������� ������!")
            {
                return RedirectToPage("/Index");
            }
            return Page(); 
        }
    }
}
