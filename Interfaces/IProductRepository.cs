using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> FindByTextAsync(string findText);
        Task<Product> GetByIdAsyncNoTracking(int id);
        bool Add(Product product);
        bool Update(Product product);
        bool Delete(Product product);
        bool Save();
        Task<IEnumerable<Product>> GetAllSort();
        Task<IEnumerable<Product>> GetProductSale();
        Task<IEnumerable<ReviewProduct>> GetReviewProductByProductId(int productId);
        Task<IEnumerable<Product>> GetProductByProductCategory(string alias, int? id);
    }
}
