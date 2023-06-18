using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Models.Entities;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ShoppingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Dependency Injection
        ShoppingContext _shoppingContext;

        public HomeController(ILogger<HomeController> logger, ShoppingContext shoppingContext)
        {
            _logger = logger;
            _shoppingContext = shoppingContext;
        }

        public async Task<IActionResult> Index(int pageNo = 1, int? categoryId = null, string? searchTerm = "")
        {
            IEnumerable<Product> products = Enumerable.Empty<Product>();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Search with predicate
                Expression<Func<Product, bool>> searchExpression = product =>
                    product.Name.ToLower().Contains(searchTerm.ToLower()) 
                    || product.Owner.FullName.ToLower().Contains(searchTerm.ToLower())
                    || product.Category.Name.ToLower().Contains(searchTerm.ToLower());
                products = await _shoppingContext.Products.AsNoTracking()
                                         .Where(searchExpression)
                                         .Include(p => p.Owner)
                                         .Include(p => p.Category)
                                         .ToListAsync();
                ViewBag.IsSearched = true;
            }
            else
            {
                // in request, if category id is null, get all products.
                // else, filter by category id
                products = categoryId == null ? await _shoppingContext.Products
                                                                      .Include(p => p.Owner)
                                                                      .Include(p => p.Category)
                                                                      .ToListAsync()
                                              : await _shoppingContext.Products
                                                                      .Where(p => p.CategoryId == categoryId)
                                                                      .Include(p => p.Owner)
                                                                      .Include(p => p.Category)
                                                                      .ToListAsync();
                ViewBag.IsSearched = false;
            }
            // pagination section
            var productCount = products.Count();
            var productPerPage = 4;

            var pagingInfo = new PagingInfo()
            {
                CurrentPage = pageNo,
                ItemsPerPage = productPerPage,
                TotalItems = productCount
            };

            var paginatedProducts = products.OrderBy(b => b.Id)
                                         .Skip((pageNo - 1) * productPerPage)
                                         .Take(productPerPage).ToList();
            ViewBag.CategoryId = categoryId;
            var model = new PaginationProductViewModel()
            {
                Products = paginatedProducts,
                PagingInfo = pagingInfo
            };
            return View(model);
        }

        public async Task<IActionResult> ProductDetail(int productId)
        {
            var product = await _shoppingContext.Products.Include(p => p.Category)
                                                         .Include(p => p.Owner)
                                                         .FirstOrDefaultAsync(p => p.Id == productId);
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}