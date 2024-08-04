using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
