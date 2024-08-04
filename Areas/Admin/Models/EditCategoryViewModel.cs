using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Areas.Admin.Models
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Alias { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
