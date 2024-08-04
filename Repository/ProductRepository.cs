using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Product product)
        {
            _context.Add(product);
            return Save();
        }

        public bool Delete(Product product)
        {
            _context.Remove(product);
            return Save();
        }

        public async Task<IEnumerable<Product>> FindByTextAsync(string findText)
        {
            return await _context.Products.Where(n => n.Title.Contains(findText) || n.Alias.Contains(findText)).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImage)
                                 .Include(p=>p.WishLists)
                                 .OrderByDescending(n => n.Id)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllSort()
        {
            return await _context.Products
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImage)
                                 .Include(p => p.WishLists)
                                 .OrderByDescending(p => p.Id)
                                 .Where(p=>p.IsActive)
                                 .Take(10)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductSale()
        {
            return await _context.Products
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImage)
                                 .Include(p => p.WishLists)
                                 .Where(p => p.IsSale && p.IsActive)
                                 .OrderByDescending (p => p.CreatedDate)
                                 .Take(10)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductByProductCategory(string alias,int? id)
        {
            if (id.HasValue && id > 0)
            {
                // Lọc sản phẩm dựa trên id của danh mục sản phẩm trước khi chuyển đổi sang danh sách
                return await _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductImage)
                    .Include(p => p.WishLists)
                    .Where(x => x.ProductCategoryID == id)
                    .ToListAsync();
            }

            // Trả về tất cả các sản phẩm nếu id không hợp lệ hoặc không có giá trị
            return await _context.Products.ToListAsync();
        }
        public async Task<IEnumerable<ReviewProduct>> GetReviewProductByProductId(int productId)
        {
            return await _context.reviewProducts
                                .Where(rp => rp.ProductId == productId).ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p=>p.ProductCategory)
                .Include(p => p.ProductImage)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Product> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Products.AsNoTracking()
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImage)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Product product)
        {
            _context.Update(product);
            return Save();
        }
    }
}
