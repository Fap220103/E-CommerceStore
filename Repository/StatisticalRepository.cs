using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;


namespace ShoppingOnline.Repository
{
    public class StatisticalRepository : IStatisticalRepository
    {
        private readonly ApplicationDbContext _context;

        public StatisticalRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetStatisticalAsync(string fromDate, string toDate)
        {
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderID
                        join p in _context.Products on od.ProductID equals p.Id
                        select new
                        {
                            CreateDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreateDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                endDate = endDate.AddDays(1);
                query = query.Where(x => x.CreateDate < endDate);
            }
            //truncate time bo thoi gian chi lay ngay thang nam
            var result = await query.GroupBy(x =>x.CreateDate.Date)
                .Select(x => new
                {
                    Date = x.Key,
                    TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                    TotalSell = x.Sum(y => y.Quantity * y.Price),

                }).Select(x => new
                {
                    Date = x.Date,
                    DoanhThu = x.TotalSell,
                    LoiNhuan = x.TotalSell - x.TotalBuy,
                }).ToListAsync();
            return new JsonResult(new { Data = result });
        }
        public async Task<IActionResult> GetDonutStatisAsync()
        {
            // Truy vấn sản phẩm được mua nhiều nhất
            var productSalesData = _context.OrderDetails
                .Include(p=>p.Product)
                .GroupBy(od => od.Product.Id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.Quantity)
                .Take(10)
                .ToList();

            var labels = productSalesData.Select(x => x.ProductId).ToList();
            var data = productSalesData.Select(x => x.Quantity).ToList();

            return new JsonResult(new { labels, data });
        }
    }
}
