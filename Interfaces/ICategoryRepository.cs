using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAll();
		Task<Category> GetByIdAsync(int id);
		Task<Category> GetByIdAsyncNoTracking(int id);
		bool Add(Category category);
		bool Update(Category category);
		bool Delete(Category category);
		bool Save();
	}
}
