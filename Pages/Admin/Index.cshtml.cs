using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using GlassStore.Entities;

namespace GlassStore.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;

        public IndexModel(GlassesStoreDbContext context)
        {
            _context = context;
        }

        // ================= DASHBOARD DATA =================
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // (Optional nâng cao)
        public decimal RevenueToday { get; set; }

        // ================= AUTH =================
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var role = HttpContext.Session.GetString("ROLE");

            if (role != "Admin")
            {
                context.Result = RedirectToPage("/Login/Login");
            }
        }
        // ================= LOAD DATA =================
        public async Task OnGetAsync()
        {
            // 1. Chỉ đếm các sản phẩm CHƯA bị xóa mềm
            TotalProducts = await _context.Products
                .Where(p => !p.IsDeleted)
                .CountAsync();

            // 2. Chỉ tính tổng kho của các sản phẩm C
            TotalStock = await _context.Products
                     .Where(p => !p.IsDeleted)
                         .SumAsync(p => p.Stock);

            // 3. Đơn hàng và doanh thu giữ nguyên (vì sản phẩm xóa rồi nhưng tiền vẫn đã thu)
            TotalOrders = await _context.Orders.CountAsync();

            TotalRevenue = await _context.OrderDetails
                .SumAsync(od => (decimal?)(od.Quantity * od.Price)) ?? 0;

            RevenueToday = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.HasValue &&
                             od.Order.OrderDate.Value.Date == DateTime.Today)
                .SumAsync(od => (decimal?)(od.Quantity * od.Price)) ?? 0;
        }
    }
}