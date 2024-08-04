using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingOnline.Models;
using System.ComponentModel.DataAnnotations;


namespace ShoppingOnline.Areas.Admin.Models
{
    public class AddProductViewModel
    {
        public int Id { get; set; }
      
        public string Title { get; set; }
    
        public string? Alias { get; set; }
     
        public string? ProductCode { get; set; }

        public string? Description { get; set; }
      
        public string? Detail { get; set; }
       
        public string? Image { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int Quantity { get; set; }
        public int? ViewCount { get; set; }
        public bool IsHome { get; set; }
        public bool IsSale { get; set; }
        public bool IsFeature { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        public int ProductCategoryID { get; set; }
      
        public string? SeoTitle { get; set; }
     
        public string? SeoDescription { get; set; }
   
        public string? SeoKeywords { get; set; }

		public IEnumerable<SelectListItem>? ProductCategory { get; set; }
        public IEnumerable<ProductImage>? ProductImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
