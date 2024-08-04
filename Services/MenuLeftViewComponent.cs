using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;

namespace ShoppingOnline.Services
{
    public class MenuLeftViewComponent : ViewComponent
    {
        private readonly IMenuRepository _menuRepository;

        public MenuLeftViewComponent(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var productCategories = await _menuRepository.GetProductCategoriesAsync();
            return View(productCategories);
        }
    }
}
