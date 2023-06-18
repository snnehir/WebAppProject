using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Models.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ShoppingContext _shoppingContext;
        public OrderController(ShoppingContext shoppingContext)
        {
            _shoppingContext = shoppingContext;
        }
        public async Task<IActionResult> Index()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
            var model = await _shoppingContext.Orders.Where(o => o.CustomerId == user.Id)
                                                     .OrderByDescending(o => o.CreatedDate).ToListAsync();
            return View(model);
        }
        public async Task<IActionResult> ConfirmOrder()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));

            var serializedString = HttpContext.Session.GetString("basket");

            var productCollection = serializedString == null ? default
                                            : JsonSerializer.Deserialize<ProductCollectionModel>(serializedString);
            if(productCollection == null || productCollection.TotalProductsCount() == 0)
            {
                return RedirectToAction("Index", "Shopping");
            }
            // add order
            var order = new Order()
            {
                CreatedDate = DateTime.Now,
                CustomerId = user.Id,
                Status = "Pending",
                TotalPrice = productCollection.TotalPrice()
            };
            await _shoppingContext.AddAsync(order);
            await _shoppingContext.SaveChangesAsync();

            foreach (var item in productCollection.ProductItems)
            {
                order.Products.Add(new OrderDetail() 
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice
                });
            }
            await _shoppingContext.SaveChangesAsync();

            // clear session
            productCollection.ProductItems.Clear();
            var serialized = JsonSerializer.Serialize<ProductCollectionModel>(productCollection);
            HttpContext.Session.SetString("basket", serialized);
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
            var model = await _shoppingContext.Orders.Include(o => o.Products)
                                                     .ThenInclude(od => od.Product)
                                                     .FirstOrDefaultAsync(o => o.Id == id);
            //ViewBag.TotalPrice = model.TotalPrice;
            return View(model.Products);
        }
    }
}
