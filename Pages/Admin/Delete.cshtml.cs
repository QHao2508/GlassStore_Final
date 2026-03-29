using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GlassStore.Entities;

namespace GlassStore.Pages.Admin
{
    public class DeleteModel : PageModel
    {
        private readonly GlassStore.Entities.GlassesStoreDbContext _context;

        public DeleteModel(GlassStore.Entities.GlassesStoreDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);

            if (product is not null)
            {
                Product = product;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product != null)
            { 
                product.IsDeleted = true;
                _context.Attach(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
