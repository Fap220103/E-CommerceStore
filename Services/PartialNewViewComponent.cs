using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;
using System.Web.Mvc;

namespace ShoppingOnline.Services
{
    public class PartialNewViewComponent : ViewComponent
    {
     
        private readonly INewRepository _newRepository;
        public PartialNewViewComponent(INewRepository newRepository)
        {   
            _newRepository = newRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var news = await _newRepository.GetTopNews();
            return View(news);
        }
    }
}
