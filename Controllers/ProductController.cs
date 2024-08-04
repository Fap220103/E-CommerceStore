using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using PagedList.Core;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;

        public ProductController(IProductRepository productRepository,
            IProductCategoryRepository productCategoryRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
        }
        [Route("/san-pham")]
		public async Task<IActionResult> Index(string Searchtext, int? page, int pageSize = 20)
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
        [Route("/chi-tiet/{alias}-p{id}")]
        public async Task<IActionResult> Detail(string alias, int id)
        {
            var item = await _productRepository.GetByIdAsync(id);
            if (item != null)
            {
               
                item.ViewCount = item.ViewCount + 1;
                _productRepository.Update(item);
            }
            var Review = await _productRepository.GetReviewProductByProductId(id);
            var countReview = Review != null ? Review.Count() : 0;
            ViewBag.Count = countReview;
            ViewBag.CountRate = await CountRate(id);
            //ViewBag.CountRate2 = CountRate2(id);
            return View(item);
        }
        public async Task<int> CountRate(int productId)
        {
            var items = await _productRepository.GetReviewProductByProductId(productId);
            if (items.Any())
            {
                var n = items.Count();
                var totalRate = items.Sum(x => x.Rate);
                return totalRate / n;
            }
            return 0;
        }
        [Route("/danh-muc-san-pham/{alisa}-{id}")]
        public async Task<IActionResult> ProductCategory(string alias, int id)
        {
            var items = await _productRepository.GetProductByProductCategory(alias, id);
            var cate = await _productCategoryRepository.GetByIdAsync(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }
            ViewBag.CateId = id;
            return View(items);
        }
        public async Task<IActionResult> Partial_ItemsByCateId()
        {
            var items = await _productRepository.GetAllSort();
            return PartialView(items);
        }
        public async Task<IActionResult> Partial_ProductSales()
        {
            var items = await _productRepository.GetProductSale();
            return PartialView(items);
        }

    }
}
