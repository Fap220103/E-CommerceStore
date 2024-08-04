using Microsoft.AspNetCore.Mvc;

namespace ShoppingOnline.Interfaces
{
    public interface IStatisticalRepository
    {
        Task<IActionResult> GetDonutStatisAsync();
        Task<IActionResult> GetStatisticalAsync(string fromDate, string toDate);
    }
}
