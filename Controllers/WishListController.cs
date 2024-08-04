using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ShoppingOnline.Data;
using ShoppingOnline.Models;

namespace ShoppingOnline.Controllers
{
    public class WishListController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<AppUser> _signInManager;

        public WishListController(ApplicationDbContext db,
            SignInManager<AppUser> signInManager)
        {
            _db = db;
            _signInManager = signInManager;
        }
        public IActionResult Index(int? page,int pageSize = 10)
        {
            var items = _db.wishLists.Include(wl => wl.Product).Where(x => x.UserName == User.Identity.Name).ToList();
         
            var pageNumber = page ?? 1;
            // Phân trang các mục
            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PagedList
            var pagedList = new StaticPagedList<WishList>(pagedItems, pageNumber, pageSize, items.Count());
            return View(pagedList);

        }
        [HttpPost]
        public IActionResult PostWishList(int ProductId)
        {
            if (_signInManager.IsSignedIn(User) == false)
            {
                return new JsonResult(new { Success = false, Message = "Bạn chưa đăng nhập" });
            }

            var item = new WishList();
            item.ProductId = ProductId;
            item.UserName = User.Identity.Name;
            item.CreateDate = DateTime.Now;
            _db.wishLists.Add(item);
            _db.SaveChanges();
            return new JsonResult(new { Success = true, Message = "Thêm sản phẩm yêu thích thành công" });
        }
        [HttpPost]
        public IActionResult PostDeleteWishList(int ProductId)
        {
            if (_signInManager.IsSignedIn(User) == false)
            {
                return new JsonResult(new { Success = false, Message = "Bạn chưa đăng nhập" });
            }
            var checkItem = _db.wishLists.FirstOrDefault(x => x.ProductId == ProductId && x.UserName == User.Identity.Name);
            if (checkItem != null)
            {
                _db.wishLists.Remove(checkItem);
                _db.SaveChanges();
                return new JsonResult(new { Success = true, Message = "Xóa thành công" });
            }
            return new JsonResult(new { Success = false, Message = "Xóa thất bại" });
        }
        
    }
}
