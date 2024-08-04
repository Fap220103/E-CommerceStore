using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Extentions;
using ShoppingOnline.Helpers;
using ShoppingOnline.Models;
using ShoppingOnline.ViewModels;
using System.Security.Claims;
using System.Web.Helpers;


namespace ShoppingOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
		private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly VnPayLibrary _vnPayLibrary;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartController(ApplicationDbContext db,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            VnPayLibrary vnPayLibrary,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
			_userManager = userManager;
            _configuration = configuration;
            _vnPayLibrary = vnPayLibrary;
            _httpContextAccessor = httpContextAccessor;
        }
        [Route("/gio-hang")]
        public IActionResult Index()
        {
            //var a = UrlPayment(2, "DH0623");
            ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
       
        public IActionResult Show_Count()
        {
             ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
            if (cart != null)
            {
                return new JsonResult(new { Count = cart.Items.Count });
            }
            return new JsonResult(new { Count = 0 });
        }
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, code = -1, Count = 0 };
         
            var checkProduct = _db.Products.Include(p => p.ProductCategory)
                                            .Include(p => p.ProductImage)
                                            .FirstOrDefault(x => x.Id == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.Id,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.ProductCategory.Title,
                    Quantity = quantity,
                    Alias = checkProduct.Alias,

                };
                if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                {
                    item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                }
                item.Price = checkProduct.Price;
                if (checkProduct.PriceSale > 0)
                {
                    item.Price = (decimal)checkProduct.PriceSale;
                }
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                HttpContext.Session.SetObject("Cart", cart);
                code = new { Success = true, code = 1, Count = cart.Items.Count };

            }
            return new JsonResult(code);
        }
		[Route("thanh-toan")]
		[HttpGet]
		public IActionResult CheckOut()
        {
            ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
			if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        [Route("thanh-toan")]
        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CheckOut(OrderViewModel req)
		{
			var code = new { Success = false, code = -1, Url = "" };
			if (ModelState.IsValid)
			{
				ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
				if (cart != null && cart.Items.Any())
				{
					Order order = new Order();
					order.CustomerName = req.CustomerName;
					order.Phone = req.Phone;
					order.Address = req.Address;
					order.Email = req.Email;
					//1: chua thanh toan
					//2: da thanh toan
					//3: hoan thanh
					//4: huy
					order.Status = 1;

					cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
					{
						ProductID = x.ProductId,
						Quantity = x.Quantity,
						Price = x.Price,
					}));
					order.Quantity = cart.Items.Sum(x => x.Quantity);
					order.TotalAmount = cart.Items.Sum(x => (x.Quantity * x.Price));
					order.TypePayment = req.TypePayment;
					order.CreatedDate = DateTime.Now;
					order.CreatedBy = req.Phone;
					order.ModifiedDate = DateTime.Now;
					if (User.Identity.IsAuthenticated)
					{
						order.CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
					}
					Random rd = new Random();
					order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
					_db.Orders.Add(order);
					_db.SaveChanges();
					////send mail cho khach hang
					//var strSanPham = "";
					//var thanhtien = decimal.Zero;
					//var TongTien = decimal.Zero;
					//var ngaydat = order.CreatedDate;
					//foreach (var sp in cart.Items)
					//{
					//    strSanPham += "<tr>";
					//    strSanPham += "<td>" + sp.ProductName + "</td>";
					//    strSanPham += "<td>" + sp.Quantity + "</td>";
					//    strSanPham += "<td>" + WebBanHangOnline.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
					//    strSanPham += "</tr>";
					//    thanhtien += sp.Price * sp.Quantity;
					//}
					//TongTien = thanhtien;
					//string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
					//contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
					//contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
					//contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
					//contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
					//contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
					//contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
					//contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
					//contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebBanHangOnline.Common.Common.FormatNumber(thanhtien, 0));
					//contentCustomer = contentCustomer.Replace("{{TongTien}}", WebBanHangOnline.Common.Common.FormatNumber(TongTien, 0));
					//WebBanHangOnline.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentCustomer.ToString(), req.Email);

					////send mail cho admin
					//string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
					//contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
					//contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
					//contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
					//contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
					//contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
					//contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
					//contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
					//contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebBanHangOnline.Common.Common.FormatNumber(thanhtien, 0));
					//contentAdmin = contentAdmin.Replace("{{TongTien}}", WebBanHangOnline.Common.Common.FormatNumber(TongTien, 0));
					//WebBanHangOnline.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);

					cart.ClearCart();
					HttpContext.Session.SetObject("Cart", cart);
					code = new { Success = true, code = req.TypePayment, Url = "" };
                    //var url = "";
                    if (req.TypePayment == 2)
                    {

                        var url = UrlPayment(req.TypePaymentVN, order.Code);
                        code = new { Success = true, code = req.TypePayment, Url = url };
                    }



                    //return RedirectToAction("CheckOutSuccess");
                    //return Json(code);
                }
			}
			return new JsonResult(code);
		}
        #region thanh toan vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = _db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = _configuration["VNPay:vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["VNPay:vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["VNPay:vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"]; //Secret Key
                                                                            //Build URL for VNPAY


            _vnPayLibrary.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            _vnPayLibrary.AddRequestData("vnp_Command", "pay");
            _vnPayLibrary.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            //vnpay.AddRequestData("vnp_Amount", (order.TotalAmount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            int amount = (int)(order.TotalAmount * 100); // Convert to smallest currency unit
            _vnPayLibrary.AddRequestData("vnp_Amount", amount.ToString());
            if (TypePaymentVN == 1)
            {
                _vnPayLibrary.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                _vnPayLibrary.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                _vnPayLibrary.AddRequestData("vnp_BankCode", "INTCARD");
            }

            _vnPayLibrary.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            //string createDate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            //vnpay.AddRequestData("vnp_CreateDate", createDate);
            _vnPayLibrary.AddRequestData("vnp_CurrCode", "VND");
            _vnPayLibrary.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor));
            _vnPayLibrary.AddRequestData("vnp_Locale", "vn");
            _vnPayLibrary.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng:" + order.Code);
            _vnPayLibrary.AddRequestData("vnp_OrderType", "other"); //default value: other

            _vnPayLibrary.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            _vnPayLibrary.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = _vnPayLibrary.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return paymentUrl;
        }
        public ActionResult VnpayReturn()
        {
            if (Request.Query.Count > 0)
            {
                string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary(_httpContextAccessor);

                foreach (var kv in vnpayData)
                {
                    // Get all querystring data
                    if (!string.IsNullOrEmpty(kv.Key) && kv.Key.StartsWith("vnp_"))
                    {
                        // `kv.Value` là `StringValues` - có thể chứa nhiều giá trị
                        vnpay.AddResponseData(kv.Key, kv.Value.ToString());
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                string orderCode = vnpay.GetResponseData("vnp_TxnRef");
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.Query["vnp_SecureHash"];
                String TerminalID = Request.Query["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.Query["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        var itemOrder = _db.Orders.FirstOrDefault(o => o.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.Status = 2;

                            _db.Orders.Attach(itemOrder);
                            _db.Update(itemOrder);
                            _db.SaveChanges();
                        }
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";

                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    ViewBag.ThanhToanThanhCong = "So tien thanh toan (VND):" + vnp_Amount.ToString();
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    //displayAmount.InnerText = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }

            }
            return View();
        }
        #endregion
        public IActionResult CheckOutSuccess()
        {
            return View();
        }
      
		[HttpPost]
		public IActionResult Delete(int id)
		{
			var code = new { Success = false, code = -1, Count = 0 };
			ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
			if (cart != null)
			{
				var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
				if (checkProduct != null)
				{
					cart.Remove(id);
					code = new { Success = true, code = 1, Count = cart.Items.Count };
				}
			}
			HttpContext.Session.SetObject("Cart", cart);
			return new JsonResult(code);
		}
		[HttpPost]
		public IActionResult DeleteAll()
		{
			ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
			if (cart != null)
			{
				cart.ClearCart();
                HttpContext.Session.SetObject("Cart", cart);
                return new JsonResult(new { Success = true });
			}
         
            return new JsonResult(new { Success = false });
		}
		[HttpPost]
		public IActionResult Update(int id, int quantity)
		{
			ShoppingCart cart = HttpContext.Session.GetObject<ShoppingCart>("Cart");
			if (cart != null)
			{
				cart.UpdateQuantity(id, quantity);
                HttpContext.Session.SetObject("Cart", cart);
                return new JsonResult(new { Success = true });
			}
          
            return new JsonResult(new { Success = false });
		}
        public IActionResult Partial_Item_Cart()
        {
            return ViewComponent("PartialCart");
        }
      
    }
}
