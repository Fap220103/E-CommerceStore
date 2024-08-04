using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
    public class ProductImageRepository : IProductImageRepository
	{
		private readonly ApplicationDbContext _context;

		public ProductImageRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<bool> AddAsync(ProductImage productImage)
		{
			_context.Add(productImage);
			return await SaveAsync();
		}
        public bool Add(ProductImage productImage)
        {
            _context.Add(productImage);
            return Save();
        }

        public bool Delete(ProductImage productImage)
		{
			_context.Remove(productImage);
			return Save();
		}
        public async Task<bool> DeleteByIdAsync(int id)
        {
            // Lấy đối tượng cần xóa
            var item = await GetByIdAsyncNoTracking(id);

            // Kiểm tra nếu đối tượng không tồn tại
            if (item == null)
            {
                return false; // Hoặc có thể throw một exception tùy theo nhu cầu
            }

            // Xóa đối tượng
            _context.Remove(item);

            // Lưu thay đổi
            return await SaveAsync();
        }


        public async Task<IEnumerable<ProductImage>> GetAll()
		{
			return await _context.ProductImages.OrderByDescending(n => n.Id).ToListAsync();
		}
		public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int id)
		{
			return await _context.ProductImages.Where(n => n.ProductId == id).ToListAsync();
		}

		public async Task<ProductImage> GetByIdAsync(int id)
		{
			return await _context.ProductImages.FirstOrDefaultAsync(n => n.Id == id);
		}

		public async Task<ProductImage> GetByIdAsyncNoTracking(int id)
		{
			return await _context.ProductImages.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
        public async Task<bool> SaveAsync()
        {
            // Lưu thay đổi và trả về kết quả
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
