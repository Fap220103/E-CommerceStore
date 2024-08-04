using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Posts Post)
        {
            _context.Add(Post);
            return Save();
        }

        public bool Delete(Posts Post)
        {
            _context.Remove(Post);
            return Save();
        }

        public async Task<IEnumerable<Posts>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Posts> GetByIdAsync(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<Posts> GetByStringAsync(string alias)
        {
            return await _context.Posts.FirstOrDefaultAsync(n => n.Alias == alias);
        }

        public async Task<Posts> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<IEnumerable<Posts>> FindByTextAsync(string findText)
        {
            return await _context.Posts.Where(n => n.Title.Contains(findText) || n.Alias.Contains(findText)).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Posts Post)
        {
            _context.Update(Post);
            return Save();
        }
    }
}
