using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;

namespace ShoppingOnline.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuRepository _menuRepository;

        public MenuController(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        

        public async Task<IActionResult> _MenuProductCategory()
        {
            var items = await _menuRepository.GetProductCategoriesAsync();
            return PartialView("_MenuProductCategory", items);
        }
        
        public async Task<IActionResult> MenuArrivals()
        {
            var items = await _menuRepository.GetProductCategoriesAsync();
            return PartialView("_MenuArrivals", items);
        }
    }
}
