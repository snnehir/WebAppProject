using ShoppingApp.Models.Entities;

namespace ShoppingApp.Models
{
    public class PaginationProductViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
