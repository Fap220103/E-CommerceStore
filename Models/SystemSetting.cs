using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    [Table("tb_SystemSetting")]
    public class SystemSetting
    {
        [Key]
        [StringLength(50)]
        public string SettingKey { get; set; }
        [StringLength(4000)]
        public string SettingValue { get; set; }
        [StringLength(4000)]
        public string? SettingDescription { get; set; }
    }
}
