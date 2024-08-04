using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Models;

namespace ShoppingOnline.Services
{
    public class PartialSubViewComponent : ViewComponent
    {
        public PartialSubViewComponent()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sub = new Subscribe();
            return View(sub);
        }
    }
}
