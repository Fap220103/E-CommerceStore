using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Contact contact)
        {
            _context.Add(contact);
            return Save();
        }

        public bool Delete(Contact contact)
        {
            _context.Remove(contact);
            return Save();
        }

        public async Task<IEnumerable<Contact>> GetAll()
        {
            return await _context.Contacts.ToListAsync();
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            return await _context.Contacts.FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<IEnumerable<Contact>> FindByEmailAsync(string findText)
        {
            return await _context.Contacts.Where(n => n.Email.Contains(findText)).ToListAsync();
        }


        public async Task<Contact> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Contacts.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Contact contact)
        {
            _context.Update(contact);
            return Save();
        }
    }
}
