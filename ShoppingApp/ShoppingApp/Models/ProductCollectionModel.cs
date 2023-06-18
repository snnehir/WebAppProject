using ShoppingApp.Models.Entities;

namespace ShoppingApp.Models
{
    public class ProductCollectionModel
    {
        public List<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
        public void ClearAll() => ProductItems.Clear();
        public decimal TotalPrice() => ProductItems.Sum(p => (decimal)p.Product.UnitPrice * p.Quantity);

        public int TotalProductsCount() => ProductItems.Sum(p => p.Quantity);

        public void AddNewProduct(ProductItem product)
        {
            var exists = ProductItems.Any(p => p.Product.Id == product.Product.Id);
            if (exists)
            {
                var existingProduct = ProductItems.FirstOrDefault(p => p.Product.Id == product.Product.Id);
                existingProduct.Quantity += product.Quantity;
            }
            else
            {
                ProductItems.Add(product);
            }
        }
    }
    public class ProductItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
