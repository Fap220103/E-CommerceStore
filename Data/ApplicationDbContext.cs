using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Models;
using System.Reflection.Emit;

namespace ShoppingOnline.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<WishList> wishLists { get; set; }
        public DbSet<ReviewProduct> reviewProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Adv> Advs { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderID, od.ProductID });
            modelBuilder.Entity<OrderDetail>()
                .HasOne(p => p.Product)
                .WithMany(od => od.OrderDetails)
                .HasForeignKey(od => od.ProductID);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(o => o.Order)
                .WithMany(od => od.OrderDetails)
                .HasForeignKey(od => od.OrderID);

            modelBuilder.Entity<AppUser>()
             .Ignore(p => p.RoleId);

            modelBuilder.Entity<AppUser>()
                .Ignore(p => p.Role);

            modelBuilder.Entity<AppUser>()
                .Ignore(p => p.RoleList);

        }
    }
}