using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{
    public partial class UserValidation
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [StringLength(254, ErrorMessage = "Email слишком длинный (максимум 254 символа)")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полное имя обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 100 символов")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Неверный формат телефона (допустимы цифры, +, -, () и пробелы)")]
        [StringLength(20, ErrorMessage = "Телефон слишком длинный (максимум 20 символов)")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "Адрес слишком длинный (максимум 200 символов)")]
        public string? Address { get; set; }

        public string? Password { get; set; }
    }
}
