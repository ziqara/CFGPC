using DDMLib;
using System.ComponentModel.DataAnnotations;
namespace WebApplication1
{
    public partial class UserValidation
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "Полное имя обязательно")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Неверный формат телефона (только цифры и символы как +, -, ())")]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
