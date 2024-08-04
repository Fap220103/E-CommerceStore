using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductImageController : Controller
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IPhotoService _photoService;
        private readonly INotyfService _notyfService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageController(IProductImageRepository productImageRepository,
            IPhotoService photoService,
            INotyfService notyfService,
            IUnitOfWork unitOfWork)
        {
            _productImageRepository = productImageRepository;
            _photoService = photoService;
            _notyfService = notyfService;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int proId)
        {
            ViewBag.Id = proId;
            var items = await _productImageRepository.GetByProductIdAsync(proId);

            return View(items);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await _productImageRepository.DeleteByIdAsync(id);
            return new JsonResult(new { success = true  });
        }
        [HttpPost]
        public async Task<IActionResult> AddImage(int productId, IFormFile image)
        {
            var result = await _photoService.AddPhotoAsync(image);
            var item = new ProductImage
            {
                ProductId = productId,
                Image = result.Url.ToString(),
                IsDefault = false,
            };
            if (_productImageRepository.Add(item))
            {
               
                return new JsonResult(new { success = true, newId = item.Id, imageUrl = item.Image });
            }
            else
            {
               
                return new JsonResult(new { success = false});
            }

        }
        [HttpPost]
        public async Task<IActionResult> ChangeDefault(int id, int idDefault)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (idDefault != -1)
                {
                    var itemDefault = await _unitOfWork.ProductImages.GetByIdAsync(idDefault);
                    if (itemDefault != null)
                    {
                        itemDefault.IsDefault = false;
                    }
                }

                var item = await _unitOfWork.ProductImages.GetByIdAsync(id);
                item.IsDefault = true;

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
