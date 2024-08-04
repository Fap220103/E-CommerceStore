using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ShoppingOnline.Models
{
    [Table("tb_Posts")]
    public class Posts : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string? Title { get; set; }
        [StringLength(150)]
        public string? Alias { get; set; }
        public string? Description { get; set; }
        [AllowHtml]
        public string? Detail { get; set; }
        [StringLength(250)]
        public string? Image { get; set; }
        public int CategoryID { get; set; }
        [StringLength(250)]
        public string? SeoTitle { get; set; }
        [StringLength(500)]
        public string? SeoDescription { get; set; }
        [StringLength(250)]
        public string? SeoKeywords { get; set; }
        public bool IsActive { get; set; }
        public virtual Category Category { get; set; }
    }
}
