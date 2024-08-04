using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
	public interface IProductImageRepository
	{
		Task<IEnumerable<ProductImage>> GetAll();
		Task<ProductImage> GetByIdAsync(int id);
		Task<ProductImage> GetByIdAsyncNoTracking(int id);
		bool Add(ProductImage productImage);
		
		bool Delete(ProductImage productImage);
		bool Save();
		Task<IEnumerable<ProductImage>> GetByProductIdAsync(int id);
        bool DeleteById(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<bool> SaveAsync();
   
        Task<bool> AddAsync(ProductImage productImage);
    }
}
