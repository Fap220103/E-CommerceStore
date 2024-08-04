using ShoppingOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace ShoppingOnline.Areas.Admin.Models
{
    public class AddNewViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? Alias { get; set; }
        public IFormFile? Image { get; set; }
        public int CategoryID { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string? Searchtext { get; set; }
		public IEnumerable<SelectListItem>? Categories { get; set; }
		//public virtual Category Category { get; set; }
	}
}
