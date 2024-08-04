using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.ViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
