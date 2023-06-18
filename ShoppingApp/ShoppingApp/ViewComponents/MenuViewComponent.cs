using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;

namespace ShoppingApp.ViewComponents
{
    public class MenuViewComponent: ViewComponent
    {
        private readonly ShoppingContext _dbContext;
        public MenuViewComponent(ShoppingContext dbContext)
        {
            _dbContext = dbContext;
        }
        // side menu view component for displayin product categories
        public IViewComponentResult Invoke()
        {
            var categories = _dbContext.Categories.ToList();
            return View(categories);
        }
    }
}
