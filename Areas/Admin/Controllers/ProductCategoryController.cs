using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
		private readonly INotyfService _notyfService;

		public ProductCategoryController(IProductCategoryRepository productCategoryRepository,
            INotyfService notyfService)
        {
            _productCategoryRepository = productCategoryRepository;
			_notyfService = notyfService;
		}
        public async Task<IActionResult> Index()
        {
            var item = await _productCategoryRepository.GetAll();
            return View(item);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Alias = Helpers.Common.FilterChar(model.Title);
                _productCategoryRepository.Add(model);
				_notyfService.Success("Thêm thành công!");
				return RedirectToAction("Index");
            }
			_notyfService.Error("Thêm thất bại!");
			return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var items = await _productCategoryRepository.GetByIdAsync(id);
            return View(items);
        }
        [HttpPost]        
        public ActionResult Edit(ProductCategory model)
        {
            if (ModelState.IsValid)
            {

                model.ModifiedDate = DateTime.Now;
                model.Alias = Helpers.Common.FilterChar(model.Title);
                _productCategoryRepository.Update(model);
                _notyfService.Success("Sửa thành công!");
                return RedirectToAction("Index");
            }
            _notyfService.Error("Sửa thất bại!");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _productCategoryRepository.GetByIdAsync(id);
            if (item != null)
            {
                _productCategoryRepository.Delete(item);
                _notyfService.Success("Xóa thành công!");
                return Json(new { success = true });
            }
            _notyfService.Success("Sửa thất bại!");
            return Json(new { success = true });
        }
    }
}
