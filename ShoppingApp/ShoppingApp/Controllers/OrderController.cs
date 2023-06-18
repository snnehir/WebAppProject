using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
