using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    [Table("tb_Category")]
    public class Category : CommonAbstract
    {
        public Category()
        {
            this.News = new HashSet<News>();
            this.Posts = new HashSet<Posts>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(150)]
        public string Title { get; set; }
        public string Alias { get; set; }
        //[StringLength(150)]
        //public string TypeCode { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        [StringLength(150)]
        public string? SeoTitle { get; set; }
        [StringLength(250)]
        public string? SeoDescription { get; set; }
        [StringLength(150)]
        public string? SeoKeywords { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
    }
}
