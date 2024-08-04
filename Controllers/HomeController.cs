using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PagedList.Core;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.ViewModels;
using System.Diagnostics;
using System.Net;
using static ShoppingOnline.Controllers.HomeController;

namespace ShoppingOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
		private readonly INotyfService _notyfService;
        private readonly IMenuRepository _menuRepository;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,
            HttpClient httpClient, 
            INotyfService notyfService,
            IMenuRepository menuRepository,
            IProductRepository productRepository,
            ApplicationDbContext db)
        {
            _logger = logger;
            _httpClient = httpClient;
			_notyfService = notyfService;
            _menuRepository = menuRepository;
            _productRepository = productRepository;
            _db = db;
        }

        public async Task<IActionResult> Index()
		{
            var categories = await _menuRepository.GetCategoriesAsync(); // Phương thức này phải trả về một danh sách các category
            var productCategory = await _menuRepository.GetProductCategoriesAsync();
            var productSale = await _productRepository.GetProductSale();
            var menuVM = new MenuViewModel
            {
                MenuTops = categories,
                MenuArrivals = productCategory,
                ProductCategories = productCategory,
                SaleProducts = productSale,
            };

            return View(menuVM);
         
        }
      
        public ActionResult Partial_Sub()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Subscribe(Subscribe req)
        {
            if (ModelState.IsValid)
            {
                _db.Subscribes.Add(new Subscribe { Email = req.Email, CreatedDate = DateTime.Now });
                _db.SaveChanges();
                return new JsonResult(new { Success = true });
            }
            return View("Partial_Sub");
        }
        [HttpPost]
        public IActionResult Submit()
        {
            // Hiển thị thông báo khi một hành động được thực hiện
            _notyfService.Information("Your form has been submitted!");
            return RedirectToAction("Index");
        }
        public IActionResult Index1()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PostFormData([FromForm] string name, [FromForm] string email)
        {
            // Xử lý dữ liệu gửi đến
            return Ok(new { success = true });
        }
        public IActionResult Privacy()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Privacy(int x = 0)
        {
            _notyfService.Information("Your form has been submitted!");

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //[HttpGet("api/address/provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                string url = "https://vapi.vnappmob.com/api/province/";

                var response = await _httpClient.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.PermanentRedirect)
                {
                    var newUrl = response.Headers.Location.ToString();
                    var newResponse = await _httpClient.GetStringAsync(newUrl);
                    var provinceResponse = JsonConvert.DeserializeObject<ProvinceResponse>(newResponse);
                    var provinces = provinceResponse.provinces;

                    return Json(provinces);
                }
                else
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var root = JsonConvert.DeserializeObject<ProvinceResponse>(responseData);
                    var provinces = root.provinces;
                    
                    return Json(provinces);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            //string url = "https://vapi.vnappmob.com/api/province";
            //var response = await _httpClient.GetStringAsync(url);
            //var provinces = JsonConvert.DeserializeObject<List<Province>>(response);
            //var response = new WebClient().DownloadString(url);

            //ViewData["name"] = provinces[1].Name;
            //return Ok(provinces);
            //var data = JsonConvert.DeserializeObject<dynamic>(response);
            //var provinces = data.results.ToObject<List<Province>>();
            //return Ok(provinces);
        }

     
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var response = await _httpClient.GetStringAsync($"https://vapi.vnappmob.com/api/province/district/{provinceId}");
            
            var districts = JsonConvert.DeserializeObject<DistrictResponse>(response);
            var districtsData = districts.results;
            return Json(districtsData);
        }

        public async Task<IActionResult> GetWards(int districtId)
        {
            var response = await _httpClient.GetStringAsync($"https://vapi.vnappmob.com/api/province/ward/{districtId}");
            var wards = JsonConvert.DeserializeObject<WardResponse>(response);
            var wardsData = wards.results;
            return Json(wardsData);
        }

		public class ProvinceResponse
        {
            [JsonProperty("results")]
            public List<Province> provinces { get; set; }
            
        }
        public class DistrictResponse
        {
            [JsonProperty("results")]
            public List<District>? results { get; set; }

        }
        public class WardResponse
        {
            [JsonProperty("results")]
            public List<Ward>? results { get; set; }

        }
        public class Province
        {
            [JsonProperty("province_id")]
            public string Id { get; set; }

            [JsonProperty("province_name")]
            public string Name { get; set; }

            [JsonProperty("province_type")]
            public string Type { get; set; }
        }

        public class District
        {
            [JsonProperty("district_id")]
            public int Id { get; set; }
            [JsonProperty("district_name")]
            public string Name { get; set; }
            [JsonProperty("district_type")]
            public string Type { get; set; }
            [JsonProperty("province_id")]
            public string provinceId { get; set; }
        }

        public class Ward
        {
            [JsonProperty("district_id")]
            public string district_id { get; set; }
            [JsonProperty("ward_id")]
            public string Id { get; set; }
            [JsonProperty("ward_name")]
            public string Name { get; set; }
            [JsonProperty("ward_type")]
            public string Type { get; set; }
        }
        public IActionResult TestPage(int? page, int pageSize = 10)
        {
            var items = Enumerable.Range(1, 100).ToList(); // Ví dụ dữ liệu
            var pageNumber = page ?? 1;
            var pagedList = new StaticPagedList<int>(items.Skip((pageNumber - 1) * pageSize).Take(pageSize), pageNumber, pageSize, items.Count);
            return View(pagedList);
        }
    }
}