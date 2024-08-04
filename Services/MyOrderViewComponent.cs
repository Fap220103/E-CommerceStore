using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Data;
using ShoppingOnline.Models;

namespace ShoppingOnline.Services
{
    public class MyOrderViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public MyOrderViewComponent(ApplicationDbContext db,
            UserManager<AppUser> userManager)
        {
             _db = db;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    var items = _db.Orders.Where(x => x.CustomerId == user.Id).ToList();
                    return View(items);
                }
            }
            return View(new List<Order>());
        }
    }
}
