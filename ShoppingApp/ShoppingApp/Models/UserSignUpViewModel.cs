using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class UserSignUpViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords are not matching.")]
        public string ConfirmPassword { get; set; }
    }
}
