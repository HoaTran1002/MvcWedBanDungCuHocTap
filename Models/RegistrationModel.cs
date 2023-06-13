using System.ComponentModel.DataAnnotations;

namespace MvcWedBanDungCuHocTap.Models
{
    public class RegistrationModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "chiều dài tối thiểu 6 ký tự,và ít nhất 1 chữ hoa,1 chữ thường, 1 ký tự đặt biệt and 1 chữ số")]
        public string? Password { get; set; }
        [Required]
        [Compare("Password")]
        public string? PasswordConfirm { get; set; }
        public string? Role { get; set; }

    }
}