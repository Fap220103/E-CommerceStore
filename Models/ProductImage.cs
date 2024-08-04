using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{

    [Table("tb_ProductImage")]
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }
        public bool IsDefault { get; set; }

        public virtual Product Product { get; set; }
    }
}
