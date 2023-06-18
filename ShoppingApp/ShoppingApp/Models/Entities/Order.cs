namespace ShoppingApp.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        // navigation properties
        public User Customer { get; set; }
        public ICollection<OrderDetail> Products { get; set; } = new HashSet<OrderDetail>();
    }
}
