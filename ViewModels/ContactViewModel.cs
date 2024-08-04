using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.ViewModels
{
    public class ContactViewModel
    {
        public string? Name { get; set; }
    
        public string? Email { get; set; }
        public string? Company { get; set; }
        [Required]
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
