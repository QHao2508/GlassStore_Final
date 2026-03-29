using GlassStore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GlassStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Tiên VN
            var culture = new CultureInfo("vi-VN");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var builder = WebApplication.CreateBuilder(args);

            // Razor Pages
            builder.Services.AddRazorPages();

            // DbContext
            builder.Services.AddDbContext<GlassesStoreDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Session
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // TEST DB
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GlassesStoreDbContext>();

                try
                {
                    var count = db.Users.Count();
                    Console.WriteLine("DB CONNECT SUCCESS");
                    Console.WriteLine("Users = " + count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB CONNECT FAILED");
                    Console.WriteLine(ex.Message);
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseSession();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}