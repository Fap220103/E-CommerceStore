using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Models;
using System.Security.Claims;

namespace ShoppingOnline.Services
{
	public class ChangeProfileViewComponent : ViewComponent
	{
		private readonly UserManager<AppUser> _userManager;

		public ChangeProfileViewComponent(UserManager<AppUser> userManager)
        {
			_userManager = userManager;
		}
        public async Task<IViewComponentResult> InvokeAsync()
        {
			var claimsPrincipal = (ClaimsPrincipal)User;
			var user = await _userManager.GetUserAsync(claimsPrincipal);
			return View(user);
		}
    }
}
