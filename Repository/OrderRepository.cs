using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;

namespace ShoppingOnline.Repository
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderRepository(ApplicationDbContext context)
        {
			_context = context;
		}
        public bool Add(Order order)
		{
			_context.Orders.Add(order);
			return Save();	
		}

		public bool Delete(Order order)
		{
			_context.Orders.Remove(order);
			return Save();
		}

		public async Task<IEnumerable<Order>> FindByCodeAsync(string code)
		{
			return await _context.Orders.Where(n => n.Code.Contains(code)).ToListAsync();
		}

		public async Task<IEnumerable<Order>> GetAll()
		{
			return await _context.Orders.OrderByDescending(n => n.Id).ToListAsync();
		}

		public async Task<Order> GetByIdAsync(int id)
		{
			return await _context.Orders.FirstOrDefaultAsync(n => n.Id == id);
		}

		public async Task<Order> GetByIdAsyncNoTracking(int id)
		{
			return await _context.Orders.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
		}

		public async Task<IEnumerable<OrderDetail>> GetOrderDetailInOrder(int id)
		{
			return await _context.OrderDetails.Include(od=>od.Product).Where(od => od.OrderID == id).ToListAsync();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool Update(Order order)
		{
			_context.Update(order);
			return Save();
		}
	}
}
