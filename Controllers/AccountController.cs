using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Services;
using ShoppingOnline.ViewModels;
using System.Security.Claims;

namespace ShoppingOnline.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ISendMail _sendMail;
		private readonly ApplicationDbContext _db;
		private readonly INotyfService _notyfService;
		private readonly IPhotoService _photoService;

		public AccountController(UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager,
			RoleManager<IdentityRole> roleManager,
			ISendMail sendMail,
			ApplicationDbContext db,
			INotyfService notyfService,
			IPhotoService photoService
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_sendMail = sendMail;
			_db = db;
			_notyfService = notyfService;
			_photoService = photoService;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> Register()
		{			
			RegisterViewModel registerViewModel = new RegisterViewModel();	
			return View(registerViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			
			var returnUrl =Url.Content("~/Account/Login");
			if (ModelState.IsValid)
			{

				var existingUserWithEmail = await _userManager.FindByEmailAsync(registerViewModel.Email);
				if (existingUserWithEmail != null)
				{
					ModelState.AddModelError("Email", "Email already exists.");
				}
				else
				{
					var user = new AppUser { Email = registerViewModel.Email, UserName = registerViewModel.Email, Status = 1 };
					var result = await _userManager.CreateAsync(user, registerViewModel.Password);
					if (result.Succeeded)
					{						
						await _userManager.AddToRoleAsync(user, "Customer");					
						//await _signInManager.SignInAsync(user, isPersistent: false);
						_notyfService.Success("Đăng ký thành công!!");
						return LocalRedirect(returnUrl);
					}
					ModelState.AddModelError("Password", "User could not be created. Password not unique enough");
				}

			}
			_notyfService.Error("Đăng ký thất bại!!");
			return View(registerViewModel);
		}
		public IActionResult Login()
		{
			LoginViewModel loginViewModel = new LoginViewModel();
			return View(loginViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
				if (user != null)
				{
					var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
					if (passwordCheck)
					{
						var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
						if (result.Succeeded)
						{
							_notyfService.Success("Đăng nhập thành công");
							return RedirectToAction("Index", "Home");
						}
						else if (result.IsLockedOut)
						{
							_notyfService.Error("Tài khoản của bạn đã bị khóa.");
							return View("Logout");
						}
						else if (result.IsNotAllowed)
						{
							_notyfService.Error("Tài khoản của bạn chưa được xác nhận.");
						}
						else
						{
							_notyfService.Error("Đăng nhập thất bại. Vui lòng thử lại.");
						}
					}
					else
					{
						_notyfService.Error("Sai mật khẩu. Vui lòng thử lại.");
					}
				}
				else
				{
					_notyfService.Error("Email không tồn tại.");
				}
			}
			else
			{
				_notyfService.Error("Đăng nhập thất bại. Vui lòng kiểm tra thông tin và thử lại.");
			}
			return View(loginViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> LogOff()
		{
			await _signInManager.SignOutAsync();
			_notyfService.Success("Đăng xuất thành công.");
			return RedirectToAction("Index", "Home");
		}
		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			var user = await _userManager.GetUserAsync(User);
			return View(user);
		}
		[HttpPost]
		public async Task<IActionResult> ChangeProfile(ProfileViewModel profileVM,string id)
		{
			var user = await _userManager.FindByIdAsync(id);
            user.UserName = profileVM.UserName;
            user.PhoneNumber = profileVM.PhoneNumber;
			string existingImagePath = user.Image;
			try
			{
				// Xử lý ảnh mới (nếu có)
				if (profileVM.Image != null)
				{
					// Xóa ảnh cũ nếu có
					if (!string.IsNullOrEmpty(existingImagePath))
					{
						await _photoService.DeletePhotoAsync(existingImagePath);
					}

					// Thêm ảnh mới
					var photoResult = await _photoService.AddPhotoAsync(profileVM.Image);
					profileVM.URL = photoResult.Url.ToString(); // Cập nhật đường dẫn ảnh mới
				}
				else
				{
					// Nếu không có ảnh mới, giữ nguyên ảnh cũ
					profileVM.URL = existingImagePath;
				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { success = false });
			}
			user.Image = profileVM.URL;
			var result = await _userManager.UpdateAsync(user);	
			if (result.Succeeded)
			{
				
				return new JsonResult(new { success = true, user = user}) ;
			}
            return new JsonResult(new { success = false });
        }
		[HttpPost]
		public async Task<IActionResult> ChangePass(ChangePassViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return new JsonResult(new { success = false });
			}

			var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				return new JsonResult(new { success = true });
			}

			return new JsonResult(new { success = false});
		}

		[HttpGet]
        public IActionResult ForgotPassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackurl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                var emailContent = "Please reset your email by clicking this <a href=\"" + callbackurl + "\">link</a>";
                var emailSubject = "Reset Email Confirmation";
                bool emailSent = await _sendMail.SendMail("ForgotPassword", emailSubject, emailContent, model.Email);
                if (emailSent)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There was an error sending the email. Please try again.");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "User not found");
                    return View();
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirect = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirect);
            return Challenge(properties, provider);
        }
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnurl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var url = info.Principal.FindFirstValue(ClaimTypes.Uri);
            //Sign in the user with this external login provider, if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                //update any authentication tokens
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                if (string.IsNullOrEmpty(returnurl))
                {
                    returnurl = Url.Content("~/"); // Default to home page
                }
                return LocalRedirect(returnurl);
            }
            else
            {
                //If the user does not have account, then we will ask the user to create an account.
                //ViewData["ReturnUrl"] = returnurl;
                //ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
                //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                //return View("ExternalLoginConfirmation", new ExternalLoginViewModel { Email = email });
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                //var url = info.Principal.FindFirstValue(ClaimTypes.Uri);
                var user = new AppUser { UserName = email, Email = email, Status = 1 };
                var result2 = await _userManager.CreateAsync(user);
                if (result2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    result2 = await _userManager.AddLoginAsync(user, info);
                    if (result2.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        if (string.IsNullOrEmpty(returnurl))
                        {
                            returnurl = Url.Content("~/"); // Default to home page
                        }
                        return LocalRedirect(returnurl);
                    }
                }
                ModelState.AddModelError(string.Empty, "Error occurred while processing your request.");
                ViewData["ReturnUrl"] = returnurl;
                return View(nameof(ExternalLoginCallback));
            }
        }
        [HttpPost]
     
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string? returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //get the info about the user from external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email, Status=1 };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "Pokemon");
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                        return LocalRedirect(returnurl);
                    }
                }
                ModelState.AddModelError("Email", "Error occuresd");
            }
            ViewData["ReturnUrl"] = returnurl;
            return View(model);
        }
        public async Task<IActionResult> MyOrder()
        {
            return ViewComponent("MyOrder");
        }
		public async Task<IActionResult> PartialChangePass()
		{
			return ViewComponent("ChangePass");
		}
		public async Task<IActionResult> PartialChangeProfile()
		{
			return ViewComponent("ChangeProfile");
		}
	}
}
