using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.ViewModels
{
	public class RegisterViewModel
	{
		[Display(Name = "Email")]
		[Required(ErrorMessage = "Email is required")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Confirm password")]
		[Required(ErrorMessage = "Confirm password is required")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Password do not match")]
		public string ConfirmPassword { get; set; } = null!;
	}
}
