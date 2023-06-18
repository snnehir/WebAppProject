using System.ComponentModel.DataAnnotations;
namespace ShoppingApp.Models
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

    }
}
