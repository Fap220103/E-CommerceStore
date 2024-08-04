using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
	public interface IOrderRepository
	{
		Task<IEnumerable<Order>> GetAll();
		Task<Order> GetByIdAsync(int id);
		Task<IEnumerable<Order>> FindByCodeAsync(string code);
		Task<Order> GetByIdAsyncNoTracking(int id);
		Task<IEnumerable<OrderDetail>> GetOrderDetailInOrder(int id);
		bool Add(Order order);
		bool Update(Order order);
		bool Delete(Order order);
		bool Save();
	}
}
