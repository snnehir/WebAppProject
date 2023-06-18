using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class UserLoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
