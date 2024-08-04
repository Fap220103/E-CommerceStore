using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;
using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class PostController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostRepository _postRepository;
     
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyfService;

        public PostController(ICategoryRepository categoryRepository,
            IPostRepository postRepository,
            IPhotoService photoService,
            IMapper mapper,
            INotyfService notyfService)
        {
            _categoryRepository = categoryRepository;
            _postRepository = postRepository;
       
            _photoService = photoService;
            _mapper = mapper;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index(string Searchtext,int? page, int pageSize = 10)
        {
            var items = await _postRepository.GetAll();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = await _postRepository.FindByTextAsync(Searchtext);
            }
            var pageNumber = page ?? 1;
            // Phân trang các mục
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PagedList
            var pagedList = new StaticPagedList<Posts>(pagedItems, pageNumber, pageSize, items.Count());

            return View(pagedList);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var items = await _categoryRepository.GetAll();

            var AddPostViewModel = new AddPostViewModel { Categories = new SelectList(items, "Id", "Title") };
            return View(AddPostViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddPostViewModel postCreate)
        {
            if (ModelState.IsValid)
            {
               
                var result = await _photoService.AddPhotoAsync(postCreate.Image);
                postCreate.CreatedDate = DateTime.Now;
                postCreate.ModifiedDate = DateTime.Now;
                postCreate.Alias = Helpers.Common.FilterChar(postCreate.Title);

                var itemPost = _mapper.Map<Posts>(postCreate);
                itemPost.Image = result.Url.ToString();
                if (!_postRepository.Add(itemPost))
                {
                    //ModelState.AddModelError("", "Add category failed");
                    _notyfService.Error("Thêm thất bại!");
                    return View(postCreate);
                }

                _notyfService.Success("Thêm thành công!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            else
            {
                //ModelState.AddModelError("", "create category failed");
                _notyfService.Error("Thêm thất bại!");
                return View(postCreate);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var postFind = await _postRepository.GetByIdAsync(id);
            if (postFind == null) return View("Error");

            var items = await _categoryRepository.GetAll();
            var EditPostViewModel = _mapper.Map<EditPostViewModel>(postFind);

            var listCategory = new SelectList(items, "Id", "Title");
            EditPostViewModel.Categories = listCategory;
            EditPostViewModel.URL = postFind.Image;
            return View(EditPostViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditPostViewModel postEdit)
        {
            if (ModelState.IsValid)
            {
                var newFind = await _postRepository.GetByIdAsyncNoTracking(id);
                if (newFind != null)
                {
                    string existingImagePath = newFind.Image;
                    try
                    {
                        // Xử lý ảnh mới (nếu có)
                        if (postEdit.Image != null)
                        {
                            if (!string.IsNullOrEmpty(existingImagePath))
                            {
								// Xóa ảnh cũ
								await _photoService.DeletePhotoAsync(existingImagePath);
							}
                          

                            // Thêm ảnh mới
                            var photoResult = await _photoService.AddPhotoAsync(postEdit.Image);
                            postEdit.URL = photoResult.Url.ToString(); // Cập nhật đường dẫn ảnh mới
                        }
                        else
                        {
                            // Nếu không có ảnh mới, giữ nguyên ảnh cũ
                            postEdit.URL = existingImagePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Could not delete or upload photo");
                        return View(postEdit);
                    }
                    //var photoResult = await _photoService.AddPhotoAsync(newEdit.Image);
                    postEdit.ModifiedDate = DateTime.Now;
                    postEdit.Id = id;
                    postEdit.Alias = Helpers.Common.FilterChar(postEdit.Title);

                    var postUser = _mapper.Map<Posts>(postEdit);
                    postUser.Image = postEdit.URL;
                    _postRepository.Update(postUser);
                    _notyfService.Success("Sửa thành công!");
                    return RedirectToAction("Index", "Post", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "Category not found");
                    _notyfService.Error("Sửa thất bại");
                    return View(postEdit);
                }

            }
            else
            {
                ModelState.AddModelError("", "Failed to edit category");
                _notyfService.Success("Sửa thất bại!");
                return View("Edit", postEdit);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _postRepository.GetByIdAsync(id);
            if (item != null)
            {
                _postRepository.Delete(item);
                _notyfService.Success("Xóa thành công!");
                return Json(new { success = true });
            }
            _notyfService.Success("Xóa thất bại!");
            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> IsActive(int id)
        {
            var item = await _postRepository.GetByIdAsync(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _postRepository.Update(item);
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
                        var obj = await _postRepository.GetByIdAsyncNoTracking(Convert.ToInt32(item));
                        _postRepository.Delete(obj);
                    }
                }
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false });
        }
    }
}
