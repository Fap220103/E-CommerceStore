using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.ViewModels;

namespace ShoppingOnline.Services
{
	public class ChangePassViewComponent : ViewComponent
	{
        public ChangePassViewComponent()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var item = new ChangePassViewModel();
            return View(item); 
        }
    }
}
