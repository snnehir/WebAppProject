namespace ShoppingApp.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderDetail> Orders { get; set; } = new HashSet<OrderDetail>();
    }
}
