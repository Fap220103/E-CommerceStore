using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ShoppingOnline.Controllers;
using ShoppingOnline.Data;
using ShoppingOnline.Helpers;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.Repository;
using ShoppingOnline.Services;
using ShoppingOnline.UnitOfWork;

namespace ShoppingOnline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
            
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

			builder.Services.AddAuthentication()
			  .AddFacebook(options =>
			  {
				  options.AppId = "471125558745340";
				  options.AppSecret = "1070e832f026d6fa2b9e25dd43339146";
			  })
			  .AddGoogle(options =>
			  {
				  options.ClientId = "954120995122-5ed2tl4mf4pn0jl4o9ns64jmi7ttn4pg.apps.googleusercontent.com";
				  options.ClientSecret = "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";
			  });
			// Cấu hình cookie
			builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/identity/account/Login"; // Đường dẫn đến trang đăng nhập
                options.AccessDeniedPath = "/admin/home/AccessDenied"; // Đường dẫn khi người dùng không có quyền truy cập
            });

            // Đăng ký Razor Pages
            builder.Services.AddRazorPages();

            builder.Services.AddNotyf(options =>
            {
                options.Position = NotyfPosition.TopRight; // V? trí hi?n th? thông báo
                options.DurationInSeconds = 2;
                options.IsDismissable = true;
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thay đổi thời gian timeout nếu cần
                options.Cookie.HttpOnly = true; // Bảo vệ cookie
                options.Cookie.IsEssential = true; // Cookie cần thiết cho ứng dụng
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<INewRepository, NewRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IStatisticalRepository, StatisticalRepository>();
            builder.Services.AddScoped<IUnitOfWork, ProductUnitOfWork>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<ISendMail, SendMailService>();
            builder.Services.AddScoped<SettingHelper>();
            builder.Services.AddScoped<VnPayLibrary>();

            builder.Services.AddScoped<MenuTopViewComponent>();
            builder.Services.AddScoped<MenuLeftViewComponent>();
            builder.Services.AddScoped<MenuArrivalViewComponent>();
            builder.Services.AddScoped<PartialNewViewComponent>();
            builder.Services.AddScoped<IMenuRepository, MenuRepository>();


            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddHttpClient<HomeController>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // contents/1.jpg => Uploads/1.jpg
            // cau hinh static file
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(),"Uploads")
            //        ),
            //        RequestPath = "/contents"
            //});

            app.UseRouting();
            app.UseNotyf();  // Bat thông báo toast

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession(); // Thêm dòng này để sử dụng session
            // cau hinh router area
            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}