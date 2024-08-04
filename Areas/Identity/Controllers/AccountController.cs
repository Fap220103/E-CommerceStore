using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingOnline.Areas.Identity.Models;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;

namespace ShoppingOnline.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISendMail _sendMail;
        private readonly ApplicationDbContext _db;
        private readonly INotyfService _notyfService;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ISendMail sendMail,
            ApplicationDbContext db,
            INotyfService notyfService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _sendMail = sendMail;      
            _db = db;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            var userList = _db.AppUser.ToList();
            var roleList = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var role = roleList.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }
            return View(userList);
        
        }
        [HttpGet]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
              
            }
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));

            }
            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Employee"));

            }
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem()
            {
                Value = "Admin",
                Text = "Admin"
            });
            listItems.Add(new SelectListItem()
            {
                Value = "Customer",
                Text = "Customer"
            });
            listItems.Add(new SelectListItem()
            {
                Value = "Employee",
                Text = "Employee"
            });
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.RoleList = listItems;
            registerViewModel.ReturnUrl = returnUrl;
            return View(registerViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string? returnUrl = null)
        {
            registerViewModel.ReturnUrl = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/Identity/Account/Index");
            if (ModelState.IsValid)
            {

                var existingUserWithEmail = await _userManager.FindByEmailAsync(registerViewModel.Email);
                if (existingUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }
                else
                {
                    var user = new AppUser { Email = registerViewModel.Email, UserName = registerViewModel.UserName, Status =1};
                    var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                    if (result.Succeeded)
                    {
                        if (registerViewModel.RoleSelected != null && registerViewModel.RoleSelected.Length > 0)
                        {
                            await _userManager.AddToRoleAsync(user, registerViewModel.RoleSelected);
                        }
                     
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _notyfService.Success("Thêm thành công!!");
                        return LocalRedirect(returnUrl);
                    }
                    ModelState.AddModelError("Password", "User could not be created. Password not unique enough");
                }

            }
            _notyfService.Error("Thêm thất bại!!");
            return View(registerViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = _db.AppUser.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            var userRole = _db.UserRoles.ToList();//lay trong bang n-n
            var roles = _db.Roles.ToList();//lay trong bang role 1
            var role = userRole.FirstOrDefault(u => u.UserId == userId);
            if (role != null)
            {
                user.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            user.RoleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (ModelState.IsValid)
            {
                var userDbValue = _db.AppUser.FirstOrDefault(u => u.Id == user.Id);
               
                if (userDbValue == null)
                {
                    return NotFound();
                }
                userDbValue.UserName = user.UserName;           
                userDbValue.PhoneNumber = user.PhoneNumber;
                var result = await _userManager.UpdateAsync(userDbValue);
                if (result.Succeeded)
                {
                    var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == userDbValue.Id);
                    if (userRole != null)
                    {
                        var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                        await _userManager.RemoveFromRoleAsync(userDbValue, previousRoleName);
                    }
                    await _userManager.AddToRoleAsync(userDbValue, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                    _db.SaveChanges();
                    _notyfService.Success("Sửa thành công!!");
                    return RedirectToAction(nameof(Index));
                }
               
            }
            user.RoleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
			_notyfService.Error("Sửa thất bại!!");
			return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string email)
        {
            var code = new { Success = false };

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roleForUser = await _userManager.GetRolesAsync(user);
                if (roleForUser.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roleForUser); // Xóa tất cả các vai trò của người dùng
                }

                var res = await _userManager.DeleteAsync(user);
                code = new { Success = res.Succeeded };
            }

            return new JsonResult(code);
        }
		[HttpPost]
		public async Task<IActionResult> UpdateTT(string email, int trangthai)
		{
            
			var item = await _userManager.FindByEmailAsync(email);
			if (item != null)
			{
				item.Status = trangthai;
				var result = await _userManager.UpdateAsync(item);
                if (result.Succeeded)
                {
					return new JsonResult(new { success = true, statusUser = trangthai });
				}
			
			}
			return new JsonResult(new { success = false });
		}
		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			LoginViewModel loginViewModel = new LoginViewModel();
			loginViewModel.ReturnUrl = returnUrl ?? Url.Action("~/");
			return View(loginViewModel);
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                if (user != null)
                {
                    var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                    if (passwordCheck)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user.Email, loginViewModel.Password, false,false);
                        if (result.Succeeded)
                        {
                            _notyfService.Success("Đăng nhập thành công");
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
			_notyfService.Success("Đăng xuất thành công.");
			return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
