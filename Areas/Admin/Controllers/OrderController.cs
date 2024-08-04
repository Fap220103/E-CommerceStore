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
	public class OrderController : Controller
	{
		private readonly IOrderRepository _orderRepository;

		public OrderController(IOrderRepository orderRepository)
        {
			_orderRepository = orderRepository;
		}
        public async Task<IActionResult> Index(string Searchtext,int? page,int pageSize=10)
		{
			var orders = await _orderRepository.GetAll();
			if (!string.IsNullOrEmpty(Searchtext))
			{
				orders = await _orderRepository.FindByCodeAsync(Searchtext);
			}
			var pageNumber = page ?? 1;
			// Phân trang các mục
			var pagedItems = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

			// Tạo đối tượng PagedList
			var pagedList = new StaticPagedList<Order>(pagedItems, pageNumber, pageSize, orders.Count());

			return View(pagedList);
		}
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _orderRepository.GetByIdAsync(id);
            return View(item);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateTT(int id, int trangthai)
        {
            var item = await _orderRepository.GetByIdAsyncNoTracking(id);
            if (item != null)
            {            
                item.Status = trangthai;
               _orderRepository.Update(item);
                return new JsonResult(new { Success = true, status = trangthai });
            }
            return new JsonResult(new { Success = false });
        }
    }
}
