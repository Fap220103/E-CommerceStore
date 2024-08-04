using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;
using System.Web.Mvc;

namespace ShoppingOnline.Services
{
    public class MenuArrivalViewComponent : ViewComponent
    {
     
        private readonly IProductRepository _productRepository;

        public MenuArrivalViewComponent(IProductRepository productRepository)
        {
            
            _productRepository = productRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            var items = await _productRepository.GetAllSort();
            return View(items);
        }
    }
}
