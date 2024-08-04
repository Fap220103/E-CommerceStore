using ShoppingOnline.Data;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Repository;
using System;

namespace ShoppingOnline.UnitOfWork
{
    public class ProductUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProductRepository _products;
        private IProductImageRepository _productImages;

        public ProductUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
