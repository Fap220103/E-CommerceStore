using ShoppingOnline.Models;

namespace ShoppingOnline.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductImageRepository ProductImages { get; }
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
