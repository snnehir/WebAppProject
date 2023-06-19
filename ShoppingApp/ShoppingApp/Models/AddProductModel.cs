using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class AddProductModel
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
