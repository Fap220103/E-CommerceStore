using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingOnline.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CategoryController : Controller
	{
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyfService;

        public CategoryController(ICategoryRepository categoryRepository,
            IMapper mapper,
            INotyfService notyfService)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _categoryRepository.GetAll();
            return View(items);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]  
        public async Task<IActionResult> Add(AddCategoryViewModel categoryCreate)
        {
            if (ModelState.IsValid)
            {
                categoryCreate.CreatedDate = DateTime.Now;
                categoryCreate.ModifiedDate = DateTime.Now;
                categoryCreate.Alias = Helpers.Common.FilterChar(categoryCreate.Title);
                var category = _mapper.Map<Category>(categoryCreate);
                if (!_categoryRepository.Add(category))
                {
                    //ModelState.AddModelError("", "Add category failed");
                    _notyfService.Error("Category added failed!");
                    return View(categoryCreate);
                }

                _notyfService.Success("Category added successfully!");
                return RedirectToAction("Index", "Category", new {area="Admin"});
            }
            else
            {
                //ModelState.AddModelError("", "create category failed");
                _notyfService.Error("Category added failed!");
                return View(categoryCreate);
            }
          
        }
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if(category == null)
            {
                return View("Error");
            }
            var categoryVM = _mapper.Map<EditCategoryViewModel>(category);
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,EditCategoryViewModel categoryEdit)
        {
            if (ModelState.IsValid)
            {
                var categoryFind = await _categoryRepository.GetByIdAsyncNoTracking(id);
                if (categoryFind != null)
                {
                    categoryEdit.ModifiedDate = DateTime.Now;
                    categoryEdit.Id = id;
                    categoryEdit.Alias = Helpers.Common.FilterChar(categoryEdit.Title);
                    var category = _mapper.Map<Category>(categoryEdit);
                    _categoryRepository.Update(category);
                    _notyfService.Success("Sửa thành công!");
                    return RedirectToAction("Index", "Category", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "Category not found");
                    _notyfService.Error("Category not found");
                    return View(categoryEdit);
                }
            }
            else
            {
                ModelState.AddModelError("", "Failed to edit category");
                _notyfService.Success("Sửa thất bại!");
                return View("Edit", categoryEdit);
            }          
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _categoryRepository.GetByIdAsync(id);
            if (item != null)
            {
                 _categoryRepository.Delete(item);
                _notyfService.Success("Xóa thành công!");
                return Json(new { success = true });
            }
            _notyfService.Success("Sửa thất bại!");
            return Json(new { success = true });
        }
    }
}
