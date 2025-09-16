using System.ComponentModel.DataAnnotations;

namespace WEB.Models.Account
{
    public class RegisterVM
    {
        [Required]
        public string UserName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler aynı değil.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
