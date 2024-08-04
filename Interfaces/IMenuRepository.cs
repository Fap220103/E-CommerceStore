using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync();
    }
}
