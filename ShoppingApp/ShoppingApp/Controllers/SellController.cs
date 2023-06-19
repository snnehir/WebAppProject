using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Models.Entities;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellController : Controller
    {
        private readonly ShoppingContext _shoppingContext;
        public SellController(ShoppingContext shoppingContext) 
        {
            _shoppingContext = shoppingContext;
        }
        public async Task<IActionResult> Products()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
            var userProducts = await _shoppingContext.Products.Where(p => p.OwnerId == user.Id)
                                                              .Include(p => p.Category).ToListAsync();
            return View(userProducts);
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<SelectListItem> categories = _shoppingContext.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            ViewBag.Categories = categories;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(AddProductModel model)
        {
            if (ModelState.IsValid)
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
                var product = new Product()
                {
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    UnitPrice = model.UnitPrice,
                    OwnerId = user.Id
                };
                await _shoppingContext.Products.AddAsync(product);
                await _shoppingContext.SaveChangesAsync();
                ViewBag.Success = "New product is added!";
            }
            
            return View();
        }
    }
}
