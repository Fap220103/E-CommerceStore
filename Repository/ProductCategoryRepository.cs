using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(ProductCategory productCategory)
        {
            _context.Add(productCategory);
            return Save();
        }

        public bool Delete(ProductCategory productCategory)
        {
            _context.Remove(productCategory);
            return Save();
        }

        public async Task<IEnumerable<ProductCategory>> GetAll()
        {
            return await _context.ProductCategories.OrderByDescending(n => n.Id).ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<ProductCategory> GetByIdAsyncNoTracking(int id)
        {
            return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(ProductCategory productCategory)
        {
            _context.Update(productCategory);
            return Save();
        }
    }
}
