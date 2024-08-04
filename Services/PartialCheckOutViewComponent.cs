using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Extentions;
using ShoppingOnline.Models;
using System.Security.Claims;

namespace ShoppingOnline.Services
{
	public class PartialCheckOutViewComponent : ViewComponent
	{
        private readonly UserManager<AppUser> _userManager;

        public PartialCheckOutViewComponent(UserManager<AppUser> userManager)
		{
            _userManager = userManager;
        }
		public async Task<IViewComponentResult> InvokeAsync()
		{
			if (User.Identity.IsAuthenticated)
			{

                // Ép kiểu User thành ClaimsPrincipal
                var claimsPrincipal = User as ClaimsPrincipal;

                // Lấy thông tin người dùng từ UserManager
                var user = await _userManager.GetUserAsync(claimsPrincipal);

                if (user != null)
				{
					ViewBag.User = user;
				}
			}

			return View();
        }
	}
}
