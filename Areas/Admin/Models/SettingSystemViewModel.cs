namespace ShoppingOnline.Areas.Admin.Models
{
    public class SettingSystemViewModel
    {
        public string SettingTitle { get; set; }
        public IFormFile? SettingLogo { get; set; }
     
        public string SettingHotline { get; set; }
        public string SettingEmail { get; set; }
        public string SettingTitleSeo { get; set; }
        public string SettingDesSeo { get; set; }
        public string SettingKeySeo { get; set; }
    }
}
