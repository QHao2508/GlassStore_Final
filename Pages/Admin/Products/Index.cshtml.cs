using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using GlassStore.Entities;

namespace GlassStore.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;

        public IndexModel(GlassesStoreDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? IsDeletedFilter { get; set; }

        // 🔐 Admin only
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var role = HttpContext.Session.GetString("ROLE");

            if (role != "Admin")
            {
                context.Result = RedirectToPage("/Login");
            }
        }

        public async Task OnGetAsync()
        {
            var query = _context.Products.AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(p => p.Name.Contains(Search));
            }

            // 🔥 Filter trạng thái
            if (IsDeletedFilter.HasValue)
            {
                query = query.Where(p => p.IsDeleted == IsDeletedFilter.Value);
            }

            // 🔥 Sort đẹp
            query = query.OrderBy(p => p.IsDeleted).ThenBy(p => p.ProductId);

            Products = await query.ToListAsync();
        }

        // ❌ Soft delete
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // ♻️ Restore
        public async Task<IActionResult> OnPostRestoreAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}