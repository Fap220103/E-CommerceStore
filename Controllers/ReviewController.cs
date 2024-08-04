using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Data;
using ShoppingOnline.Models;


namespace ShoppingOnline.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReviewController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PostReview(ReviewProduct req)
        {
            if (ModelState.IsValid)
            {
                req.CreatedDate = DateTime.Now;
                _db.reviewProducts.Add(req);
                _db.SaveChanges();
                return new JsonResult(new { Success = true });
            }
            return new JsonResult(new { Success = false });
        }
        [HttpGet]
        public IActionResult show_review(int productId)
        {
            var item = _db.reviewProducts.Where(x => x.ProductId == productId).OrderByDescending(x => x.Id).Take(5).ToList();
            if (item != null)
            {
                return new JsonResult(new { items = item });
            }
            return new JsonResult(new { items = 0 });
        }
        [HttpGet]
        public IActionResult getCountReview(int proId)
        {
            var count = _db.reviewProducts.Where(rp=>rp.ProductId==proId).Count();
            if (count != 0)
            {
                return new JsonResult(new { count = count });
            }
            return new JsonResult(new { count = 0 });
        }
        public IActionResult Partial_Load_Review(int proId)
        {
            return ViewComponent("PartialReview", proId);
        }
    }
}
