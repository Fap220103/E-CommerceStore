using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategory>> GetAll();
        Task<ProductCategory> GetByIdAsync(int id);
        Task<ProductCategory> GetByIdAsyncNoTracking(int id);
        bool Add(ProductCategory productCategory);
        bool Update(ProductCategory productCategory);
        bool Delete(ProductCategory productCategory);
        bool Save();
    }
}
