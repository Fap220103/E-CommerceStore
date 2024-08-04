
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Data;

namespace ShoppingOnline.Services
{
    public class PartialReviewViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
     

        public PartialReviewViewComponent(ApplicationDbContext db)
        {
            _db = db;
         
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var item = _db.reviewProducts.Where(x => x.ProductId == productId)
                                         .OrderByDescending(x => x.Id)
                                         .Take(5)
                                         .ToList();
            ViewBag.Count = item.Count;
            return View(item);
        }
    }
}
