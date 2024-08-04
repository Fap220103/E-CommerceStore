using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public int TypePayment { get; set; }
        public int TypePaymentVN { get; set; }
        public string? CustomerId { get; set; }
    }
}
