using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetAll();
        Task<Contact> GetByIdAsync(int id);
        Task<Contact> GetByIdAsyncNoTracking(int id);
        bool Add(Contact contact);
        bool Update(Contact contact);
        bool Delete(Contact contact);
        bool Save();
        Task<IEnumerable<Contact>> FindByEmailAsync(string findText);
    }
}
