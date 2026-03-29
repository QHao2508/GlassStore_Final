using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GlassStore.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlassStore.Pages.Admin
{
    public class OrdersModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;

        public OrdersModel(GlassesStoreDbContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            Orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.OrderId)
                .ToListAsync();
        }

        // RESET ALL ORDERS
        public async Task<IActionResult> OnPostResetAllAsync()
        {
            _context.OrderDetails.RemoveRange(_context.OrderDetails);
            _context.Orders.RemoveRange(_context.Orders);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}