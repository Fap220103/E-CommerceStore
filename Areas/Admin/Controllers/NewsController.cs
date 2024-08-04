using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;
using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class NewsController : Controller
    {
		private readonly ICategoryRepository _categoryRepository;
		private readonly INewRepository _newRepository;
		private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyfService;

        public NewsController(ICategoryRepository categoryRepository,
            INewRepository newRepository,
            IPhotoService photoService,
            IMapper mapper,
            INotyfService notyfService)
        {
			_categoryRepository = categoryRepository;
			_newRepository = newRepository;
			_photoService = photoService;
            _mapper = mapper;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index(string Searchtext,int? page,int pageSize = 10)
        {
            var items = await _newRepository.GetAllAdmin();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = await _newRepository.FindByTextAsync(Searchtext);
            }
            var pageNumber = page ?? 1;
            // Phân trang các mục
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PagedList
            var pagedList = new StaticPagedList<News>(pagedItems, pageNumber, pageSize, items.Count());

            return View(pagedList);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var items = await _categoryRepository.GetAll();
		
            var AddNewViewModel = new AddNewViewModel { Categories = new SelectList(items, "Id", "Title") };
			return View(AddNewViewModel);
        }
        [HttpPost]  
        public async Task<IActionResult> Add(AddNewViewModel newCreate)
        {
            if (ModelState.IsValid)
            {
                var image = "";
                if (newCreate.Image != null)
                {
                   var result = await _photoService.AddPhotoAsync(newCreate.Image);
                    image = result.Url.ToString();
                }
                else
                {
                    image = string.Empty;
                }
               
                newCreate.CreatedDate = DateTime.Now;
                newCreate.ModifiedDate = DateTime.Now;
                newCreate.Alias = Helpers.Common.FilterChar(newCreate.Title);
 
                var itemNew = _mapper.Map<News>(newCreate);
                itemNew.Image = image;
                if (!_newRepository.Add(itemNew))
                {
                    //ModelState.AddModelError("", "Add category failed");
                    _notyfService.Error("Thêm thất bại!");
                    return View(newCreate);
                }

                _notyfService.Success("Thêm thành công!");
                return RedirectToAction("Index", "News", new { area = "Admin" });
            }
            else
            {
                //ModelState.AddModelError("", "create category failed");
                _notyfService.Error("Thêm thất bại!");
                return View(newCreate);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var newFind = await _newRepository.GetByIdAsync(id);
            if (newFind == null) return View("Error");

            var items = await _categoryRepository.GetAll();
			var EditNewViewModel = _mapper.Map<EditNewViewModel>(newFind);

            var listCategory = new SelectList(items, "Id", "Title");
            EditNewViewModel.Categories = listCategory;
            EditNewViewModel.URL = newFind.Image;
            return View(EditNewViewModel);
		}
        [HttpPost]
        public async Task<IActionResult> Edit(int id,EditNewViewModel newEdit)
        {
            if (ModelState.IsValid)
            {
                var newFind = await _newRepository.GetByIdAsyncNoTracking(id);
                if (newFind != null)
                {
                    string existingImagePath = newFind.Image;
                    try
                    {
                        // Xử lý ảnh mới (nếu có)
                        if (newEdit.Image != null)
                        {
							if (!string.IsNullOrEmpty(existingImagePath))
							{
								await _photoService.DeletePhotoAsync(existingImagePath);
							}

							// Thêm ảnh mới
							var photoResult = await _photoService.AddPhotoAsync(newEdit.Image);
                            newEdit.URL = photoResult.Url.ToString(); // Cập nhật đường dẫn ảnh mới
                        }
                        else
                        {
                            // Nếu không có ảnh mới, giữ nguyên ảnh cũ
                            newEdit.URL = existingImagePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Could not delete or upload photo");
                        return View(newEdit);
                    }
                    //var photoResult = await _photoService.AddPhotoAsync(newEdit.Image);
                    newEdit.ModifiedDate = DateTime.Now;
                    newEdit.Id = id;
                    newEdit.Alias = Helpers.Common.FilterChar(newEdit.Title);
                    
                    var newUser = _mapper.Map<News>(newEdit);
                    newUser.Image = newEdit.URL;
                    _newRepository.Update(newUser);
                    _notyfService.Success("Sửa thành công!");
                    return RedirectToAction("Index","News",new {area = "Admin"});
                }
                else
                {
                    ModelState.AddModelError("", "Category not found");
                    _notyfService.Error("Sửa thất bại");
                    return View(newEdit);
                }
                
            }
            else
            {
                ModelState.AddModelError("", "Failed to edit category");
                _notyfService.Success("Sửa thất bại!");
                return View("Edit", newEdit);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _newRepository.GetByIdAsync(id);
            if (item != null)
            {
                _newRepository.Delete(item);
                _notyfService.Success("Xóa thành công!");
                return Json(new { success = true });
            }
            _notyfService.Success("Sửa thất bại!");
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> IsActive(int id)
        {
            var item = await _newRepository.GetByIdAsync(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _newRepository.Update(item);
                return new JsonResult(new { success = true, isActive = item.IsActive });
            }
            return new JsonResult(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> deleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = await _newRepository.GetByIdAsyncNoTracking(Convert.ToInt32(item));
                        _newRepository.Delete(obj);
                    }
                }
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false });
        }
    }
}
