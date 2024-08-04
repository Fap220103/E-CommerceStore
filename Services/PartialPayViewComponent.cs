using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Extentions;
using ShoppingOnline.Models;

namespace ShoppingOnline.Services
{
    public class PartialPayViewComponent : ViewComponent
    {
        public PartialPayViewComponent()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
            if (cart != null && cart.Items.Any())
            {
                return View(cart.Items);
            }
            return View();
        }
    }
}
