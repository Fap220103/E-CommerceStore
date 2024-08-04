using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface IPostRepository
    {
        Task<IEnumerable<Posts>> GetAll();
        Task<Posts> GetByIdAsync(int id);
        Task<Posts> GetByIdAsyncNoTracking(int id);
        bool Add(Posts post);
        bool Update(Posts post);
        bool Delete(Posts post);
        bool Save();
        Task<IEnumerable<Posts>> FindByTextAsync(string findText);
        Task<Posts> GetByStringAsync(string alias);
    }
}
