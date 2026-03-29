using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GlassStore.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlassStore.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;

        public OrderHistoryModel(GlassesStoreDbContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("USER_ID");

            if (userId == null)
            {
                Orders = new List<Order>();
                return;
            }

            Orders = await _context.Orders
                .Where(o => o.UserId == userId.Value)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.OrderId)
                .ToListAsync();
        }
    }
}