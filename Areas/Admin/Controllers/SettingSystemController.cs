using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Data;
using ShoppingOnline.Helpers;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class SettingSystemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;
		private readonly INotyfService _notyfService;
        private readonly SettingHelper _settingHelper;

        public SettingSystemController(ApplicationDbContext context,
            IPhotoService photoService,
            INotyfService notyfService,
            SettingHelper settingHelper)
        {
            _context = context;
            _photoService = photoService;
			_notyfService = notyfService;
            _settingHelper = settingHelper;
        }
        public IActionResult Index()
		{
			ViewBag.SettingTitle = _settingHelper.GetValue("SettingTitle");
			ViewBag.SettingLogo = _settingHelper.GetValue("SettingLogo");
			
			ViewBag.SettingHotline = _settingHelper.GetValue("SettingHotline");
			ViewBag.SettingEmail = _settingHelper.GetValue("SettingEmail");
			ViewBag.SettingTitleSeo = _settingHelper.GetValue("SettingTitleSeo");
			ViewBag.SettingDesSeo = _settingHelper.GetValue("SettingDesSeo");
			ViewBag.SettingKeySeo = _settingHelper.GetValue("SettingKeySeo");
			return View();
        }
        public IActionResult Partial_Setting()
        {
           
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> AddSetting(SettingSystemViewModel req)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

					string logoUrl = null;
					if (req.SettingLogo != null && req.SettingLogo.Length > 0)
					{
						var result = await _photoService.AddPhotoAsync(req.SettingLogo);
						logoUrl = result.Url.ToString();
                        
					}
					else
					{
						// Lấy giá trị logo hiện tại từ cơ sở dữ liệu
						var existingLogo = await _context.SystemSettings
							.Where(x => x.SettingKey == "SettingLogo")
							.Select(x => x.SettingValue)
							.FirstOrDefaultAsync();

						logoUrl = existingLogo;
						
					}

					await AddOrUpdateSettingAsync("SettingTitle", req.SettingTitle);
                    await AddOrUpdateSettingAsync("SettingLogo", logoUrl);
                    await AddOrUpdateSettingAsync("SettingEmail", req.SettingEmail);
                    await AddOrUpdateSettingAsync("SettingHotline", req.SettingHotline);
                    await AddOrUpdateSettingAsync("SettingTitleSeo", req.SettingTitleSeo);
                    await AddOrUpdateSettingAsync("SettingDesSeo", req.SettingDesSeo);
                    await AddOrUpdateSettingAsync("SettingKeySeo", req.SettingKeySeo);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    //return new JsonResult(new { success = true, data = req });
                    _notyfService.Success("Lưu thành công");

                    return RedirectToAction("Index", req);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
					_notyfService.Success("Lưu thất bại");
					ViewBag.SettingTitle = _settingHelper.GetValue("SettingTitle");
					ViewBag.SettingLogo = _settingHelper.GetValue("SettingLogo");
					
					ViewBag.SettingHotline = _settingHelper.GetValue("SettingHotline");
					ViewBag.SettingEmail = _settingHelper.GetValue("SettingEmail");
					ViewBag.SettingTitleSeo = _settingHelper.GetValue("SettingTitleSeo");
					ViewBag.SettingDesSeo = _settingHelper.GetValue("SettingDesSeo");
					ViewBag.SettingKeySeo = _settingHelper.GetValue("SettingKeySeo");
					return RedirectToAction("Index", req);
					//return new JsonResult(new { success = false, data = req });
				}
            }
        }

        private async Task AddOrUpdateSettingAsync(string settingKey, string settingValue)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(x => x.SettingKey == settingKey);
            if (setting == null)
            {
                setting = new SystemSetting
                {
                    SettingKey = settingKey,
                    SettingValue = settingValue
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.SettingValue = settingValue;
                _context.Entry(setting).State = EntityState.Modified;
            }
        }

    }
}
