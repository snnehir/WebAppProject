using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Models.Entities;
using System.Text.Json;

namespace ShoppingApp.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly ShoppingContext _shoppingContext;
        public ShoppingController(ShoppingContext shoppingContext)
        {
            this._shoppingContext = shoppingContext;
        }
        public IActionResult Index()
        {
            var collection = getProductCollectionFromSession();
            return View(collection);
        }
        
        public async Task<IActionResult> AddProduct(int id)
        {
            Product selectedProduct = await _shoppingContext.Products.FindAsync(id);
            var productItem = new ProductItem()
            {
                Product = selectedProduct,
                Quantity = 1
            };

            ProductCollectionModel productCollection = getProductCollectionFromSession();
            productCollection.AddNewProduct(productItem);
            saveToSession(productCollection);

            return Json(new { message = $"Product with id {id} is added to your cart." });
        }

        public IActionResult IncreaseProductQuantity(int id)
        {
            // get collection
            ProductCollectionModel productCollection = getProductCollectionFromSession();
            if(productCollection != null)
            {
                var item = productCollection.ProductItems.Any(p=>p.Product.Id == id);
                if(item)
                {
                    productCollection.ProductItems.FirstOrDefault(p => p.Product.Id == id).Quantity += 1;
                    saveToSession(productCollection);
                }
                return Json(new { message = $"Number of product with id {id} is increased." });
            }
            return Json(new { message = $"Product with id {id} is not found" });
        }

        public IActionResult DecreaseProductQuantity(int id)
        {
            ProductCollectionModel productCollection = getProductCollectionFromSession();
            if (productCollection != null)
            {
                var exists = productCollection.ProductItems.Any(p => p.Product.Id == id);
                if (exists)
                {
                    var item = productCollection.ProductItems.FirstOrDefault(p => p.Product.Id == id);
                    if (item.Quantity == 1)
                    {
                        productCollection.ProductItems.Remove(item);
                        saveToSession(productCollection);
                        return Json(new { message = $"Rlant with id {id} is removed." });
                    }
                    else
                    {
                        productCollection.ProductItems.FirstOrDefault(p => p.Product.Id == id).Quantity -= 1;
                        saveToSession(productCollection);
                        return Json(new { message = $"Number of product with id {id} is decreased." });
                    }
                    
                }
                
            }
            return Json(new { message = $"Product with id {id} is not found" });
        }

        private ProductCollectionModel getProductCollectionFromSession()
        {
            var serializedString = HttpContext.Session.GetString("basket");

            var collection = serializedString == null ? new ProductCollectionModel()
                                                      : JsonSerializer.Deserialize<ProductCollectionModel>(serializedString);
            return collection;
        }


        private void saveToSession(ProductCollectionModel courseCollection)
        {
            var serialized = JsonSerializer.Serialize<ProductCollectionModel>(courseCollection);
            HttpContext.Session.SetString("basket", serialized);
        }
        
    }
}
