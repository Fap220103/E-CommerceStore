namespace ShoppingOnline.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
        public string? URL { get; set; }

    }
}
