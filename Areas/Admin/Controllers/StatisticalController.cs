using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class StatisticalController : Controller
    {
        private readonly IStatisticalRepository _statisticalRepository;

        public StatisticalController(IStatisticalRepository statisticalRepository)
        {
            _statisticalRepository = statisticalRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetStatistical(string fromDate, string toDate)
        {
            return await _statisticalRepository.GetStatisticalAsync(fromDate, toDate);
        }
        [HttpGet]
        public async Task<IActionResult> GetDonutStatistical()
        {
            return await _statisticalRepository.GetDonutStatisAsync();
        }
    }
}
