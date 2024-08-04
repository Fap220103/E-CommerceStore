using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;

namespace ShoppingOnline.Services
{
	public class MenuTopViewComponent : ViewComponent
	{
		private readonly IMenuRepository _menuRepository;

		public MenuTopViewComponent(IMenuRepository menuRepository)
		{
			_menuRepository = menuRepository;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _menuRepository.GetCategoriesAsync();
			return View(categories);
		}
	}
}
