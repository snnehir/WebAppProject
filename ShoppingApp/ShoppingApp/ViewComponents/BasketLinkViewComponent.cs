using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Text.Json;

namespace ShoppingApp.ViewComponents
{
    public class BasketLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var serializedString = HttpContext.Session.GetString("basket");

            var model = serializedString == null ? default(ProductCollectionModel)
                                            : JsonSerializer.Deserialize<ProductCollectionModel>(serializedString);
            return View(model);
        }
    }
}
