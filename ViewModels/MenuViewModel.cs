using ShoppingOnline.Models;

namespace ShoppingOnline.ViewModels
{
    public class MenuViewModel
    {
        public IEnumerable<Category>? MenuTops { get; set; }
        public IEnumerable<ProductCategory>? ProductCategories { get; set; }
        public IEnumerable<ProductCategory>? MenuArrivals { get; set; }
        public IEnumerable<Product>? SaleProducts { get; set; }
    }
}
