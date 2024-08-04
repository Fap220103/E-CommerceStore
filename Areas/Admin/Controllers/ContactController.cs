using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ContactController : Controller
	{
        private readonly IContactRepository _contactRepository;

        public ContactController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }
        public async Task<IActionResult> Index(string Searchtext, int? page, int pageSize = 10)
        {
            var items = await _contactRepository.GetAll();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = await _contactRepository.FindByEmailAsync(Searchtext);
            }
            var pageNumber = page ?? 1;
            // Phân trang các mục
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PagedList
            var pagedList = new StaticPagedList<Contact>(pagedItems, pageNumber, pageSize, items.Count());

            return View(pagedList);
        }
        public async Task<IActionResult> View(int id)
        {
            var item = await _contactRepository.GetByIdAsync(id);
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTT(int id, bool trangthai)
        {
            var item = await _contactRepository.GetByIdAsync(id);
            if (item != null)
            {              
                item.IsRead = trangthai;
                _contactRepository.Update(item);
                return new JsonResult(new {success = true, IsRead = trangthai });
            }
            return new JsonResult(new {success = false });
        }
    }
}
