using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Data;
using ShoppingOnline.Models;
using System.Web.Mvc;

namespace ShoppingOnline.Services
{
    public class ReviewViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ReviewViewComponent(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
          
            var item = new ReviewProduct();
            item.ProductId = productId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    item.Email = user.Email;
                    item.UserName = user.UserName;
                }
            }

            return View(item);
        }
    }
}
