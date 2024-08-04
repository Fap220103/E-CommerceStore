using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ShoppingOnline.Models
{

    [Table("tb_News")]
    public class News : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Bạn không được để trống tiêu đề")]
        [StringLength(1000)]
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public string? Description { get; set; }
        [AllowHtml]
        public string? Detail { get; set; }
        public string? Image { get; set; }
        public int CategoryID { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public bool IsActive { get; set; }
        public virtual Category Category { get; set; }
    }

}
