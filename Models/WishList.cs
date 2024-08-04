using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    [Table("tb_WishList")]
    public class WishList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual Product Product { get; set; }
    }
}
