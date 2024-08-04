using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;

namespace ShoppingOnline.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewRepository _newRepository;

        public NewsController(INewRepository newRepository)
        {
            _newRepository = newRepository;
        }
        [Route("/tin-tuc")]
        public async Task<IActionResult> Index(int? page, int pageSize = 20)
        {
            var items = await _newRepository.GetAll();          
            var pageNumber = page ?? 1;
            // Phân trang các mục
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PagedList
            var pagedList = new StaticPagedList<News>(pagedItems, pageNumber, pageSize, items.Count());
            return View(pagedList);
        }
        [Route("/tin-tuc/{alias}n{id}")]
        public async Task<IActionResult> Details(string alias,int id)
        {
            var item = await _newRepository.GetByIdAsync(id);
            return View(item);
        }
        public async Task<IActionResult> Partial_News_Home()
        {
            var items = await _newRepository.GetTopNews();
            return PartialView(items);
        }
    }
}
