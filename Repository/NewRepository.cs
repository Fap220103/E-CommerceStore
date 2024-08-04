using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
    public class NewRepository : INewRepository
    {
        private readonly ApplicationDbContext _context;

        public NewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(News news)
        {
            _context.Add(news);
            return Save();
        }

        public bool Delete(News news)
        {
            _context.Remove(news);
            return Save();
        }

        public async Task<IEnumerable<News>> FindByTextAsync(string findText)
        {
            return await _context.News.Where(n=> n.Title.Contains(findText) || n.Alias.Contains(findText)).ToListAsync();
        }
		public async Task<IEnumerable<News>> GetAllAdmin()
		{
			return await _context.News.OrderByDescending(n => n.Id).ToListAsync();
		}
		public async Task<IEnumerable<News>> GetAll()
        {
            return await _context.News.Where(n=> n.IsActive).OrderByDescending(n=>n.Id).ToListAsync();
        }
        public async Task<IEnumerable<News>> GetTopNews()
        {
            return await _context.News.Where(n=>n.IsActive).OrderByDescending(n => n.CreatedDate).Take(3).ToListAsync();
         
        }

        public async Task<News> GetByIdAsync(int id)
        {
            return await _context.News.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<News> GetByIdAsyncNoTracking(int id)
        {
            return await _context.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(News news)
        {
            _context.Update(news);
            return Save();
        }
    }
}
