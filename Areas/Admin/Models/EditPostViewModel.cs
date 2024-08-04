using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Areas.Admin.Models
{
    public class EditPostViewModel
    {
        [Required]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? Alias { get; set; }
        public IFormFile? Image { get; set; }
        public string? URL { get; set; }
        public int CategoryID { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
