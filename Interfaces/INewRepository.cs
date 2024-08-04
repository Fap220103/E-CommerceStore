using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface INewRepository
    {
        Task<IEnumerable<News>> GetAll();
        Task<News> GetByIdAsync(int id);
        Task<IEnumerable<News>> FindByTextAsync(string findText);
        Task<News> GetByIdAsyncNoTracking(int id);
        bool Add(News news);
        bool Update(News news);
        bool Delete(News news);
        bool Save();
        Task<IEnumerable<News>> GetTopNews();
		Task<IEnumerable<News>> GetAllAdmin();
	}
}
