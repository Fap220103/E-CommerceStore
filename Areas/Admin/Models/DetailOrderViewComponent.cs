using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;

namespace ShoppingOnline.Areas.Admin.Models
{
    public class DetailOrderViewComponent : ViewComponent
    {
        private readonly IOrderRepository _orderRepository;

        public DetailOrderViewComponent(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var item = await _orderRepository.GetOrderDetailInOrder(id);
            return View(item);
        }
    }
}
