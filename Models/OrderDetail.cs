using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    [Table("tb_OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [Column(Order = 0)]
        public int OrderID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
