using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;
using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyfService;
        private readonly IProductCategoryRepository _productCategoryRepository;
		private readonly IPhotoService _photoService;
		private readonly IProductImageRepository _productImageRepository;
		private readonly ILogger<ProductController> _logger;

		public ProductController(IProductRepository productRepository,
            IMapper mapper,
            INotyfService notyfService,
            IProductCategoryRepository productCategoryRepository,
            IPhotoService photoService,
			IProductImageRepository productImageRepository,
			ILogger<ProductController> logger
            )
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _notyfService = notyfService;
            _productCategoryRepository = productCategoryRepository;
			_photoService = photoService;
			_productImageRepository = productImageRepository;
			_logger = logger;
		}
        public async Task<IActionResult> Index(string Searchtext,int? page,int pageSize = 10)
        {
            var items = await _productRepository.GetAll();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = await _productRepository.FindByTextAsync(Searchtext);
            }
            var pageNumber = page ?? 1;
            // Phân trang các mục
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PagedList
            var pagedList = new StaticPagedList<Product>(pagedItems, pageNumber, pageSize, items.Count());
            return View(pagedList);

        }
        public async Task<IActionResult> Add()
        {
			var productCategory = await _productCategoryRepository.GetAll();
			ViewBag.ProductCategory = new SelectList(productCategory, "Id", "Title");
			return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel addProductVM, List<IFormFile> Images,List<int> rDefault)
        {
            if(ModelState.IsValid)
            {

                addProductVM.CreatedDate = DateTime.Now;
                addProductVM.ModifiedDate = DateTime.Now;
                if (string.IsNullOrEmpty(addProductVM.SeoTitle))
                {
                    addProductVM.SeoTitle = addProductVM.Title;
                }
                if (string.IsNullOrEmpty(addProductVM.Alias))
                {
                    addProductVM.Alias = Helpers.Common.FilterChar(addProductVM.Title);
                }
                var product = _mapper.Map<Product>(addProductVM);
                _productRepository.Add(product);
                if (Images != null && Images.Count > 0)
				{
					for (int i = 0; i < Images.Count; i++)
					{
						if (i + 1 == rDefault[0])
						{
							var result = await _photoService.AddPhotoAsync(Images[i]);
							product.Image = result.Url.ToString();
							var productImage = new ProductImage
							{
								ProductId = product.Id,
								Image = result.Url.ToString(),
								IsDefault = true,
							};
							_productImageRepository.Add(productImage);
						}
						else
						{
							var result = await _photoService.AddPhotoAsync(Images[i]);
							var productImage = new ProductImage
							{
								ProductId = product.Id,
								Image = result.Url.ToString(),
								IsDefault = false,
							};
							_productImageRepository.Add(productImage);
						}
					}
				}
                _productRepository.Update(product);
                _notyfService.Success("Thêm thành công!");
				return RedirectToAction("Index", "Product", new { area = "Admin" });
			}
			foreach (var state in ModelState)
			{
				foreach (var error in state.Value.Errors)
				{
					_logger.LogError($"Property: {state.Key}, Error: {error.ErrorMessage}");
				}
			}
			var productCategory = await _productCategoryRepository.GetAll();
			ViewBag.ProductCategory = new SelectList(productCategory, "Id", "Title");
			_notyfService.Error("Thêm thất bại!");
			return View(addProductVM);
		}
		public async Task<IActionResult> Edit(int id)
		{
			var productFind = await _productRepository.GetByIdAsync(id);
			 if (productFind == null) return View("Error");

			var items = await _productCategoryRepository.GetAll();
			var EditProductVM = _mapper.Map<EditProductViewModel>(productFind);

			var listproductCategory = new SelectList(items, "Id", "Title");
			EditProductVM.ProductCategories = listproductCategory;
			
			return View(EditProductVM);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(int id, EditProductViewModel productEdit)
		{
			if (ModelState.IsValid) 
			{
				var productFind = await _productRepository.GetByIdAsyncNoTracking(id);
				if (productFind != null)
				{
					
					productEdit.ModifiedDate = DateTime.Now;
					productEdit.Id = id;
					productEdit.Alias = Helpers.Common.FilterChar(productEdit.Title);

					var productUser = _mapper.Map<Product>(productEdit);
				
					_productRepository.Update(productUser);
					_notyfService.Success("Sửa thành công!");
					return RedirectToAction("Index","Product", new {area="Admin"});
				}
				else
				{
					ModelState.AddModelError("", "Product not found");
					_notyfService.Error("Sửa thất bại");
					return View(productEdit);
				}

			}
			else
			{
				ModelState.AddModelError("", "Failed to edit product");
				_notyfService.Error("Sửa thất bại!");
				return View("Edit", productEdit);
			}
		}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _productRepository.GetByIdAsync(id);
            if (item != null)
            {
                var checkImg = item.ProductImage.ToList();
                if (checkImg != null)
                {
                    foreach (var img in checkImg)
                    {
                        _productImageRepository.Delete(img);
                    }
                }
                _productRepository.Delete(item);
                return new JsonResult(new { success = true });
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
                        var obj = await _productRepository.GetByIdAsync(Convert.ToInt32(item));
                        _productRepository.Delete(obj);
                    }
                }
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> IsActive(int id)
        {
            var item = await _productRepository.GetByIdAsync(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _productRepository.Update(item);
                return new JsonResult(new { success = true, isActive = item.IsActive });
            }
            return new JsonResult(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> IsHome(int id)
        {
            var item = await _productRepository.GetByIdAsync(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                _productRepository.Update(item);
                return new JsonResult(new { success = true, isHome = item.IsHome });
            }
            return new JsonResult(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> IsSale(int id)
        {
            var item = await _productRepository.GetByIdAsync(id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                _productRepository.Update(item);
                return new JsonResult(new { success = true, isSale = item.IsSale });
            }
            return new JsonResult(new { success = false });
        }
    }

}
