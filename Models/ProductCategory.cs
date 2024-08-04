using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    [Table("tb_ProductCategory")]
    public class ProductCategory : CommonAbstract
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        [StringLength(150)]
        public string? Alias { get; set; }
        public string? Description { get; set; }
        [StringLength(250)]
        public string? Icon { get; set; }
        [StringLength(250)]
        public string? SeoTitle { get; set; }
        [StringLength(500)]
        public string? SeoDescription { get; set; }
        [StringLength(250)]
        public string? SeoKeywords { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
