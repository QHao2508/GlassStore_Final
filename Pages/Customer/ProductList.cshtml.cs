using GlassStore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace GlassStore.Pages.Customer
{
    public class ProductListModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;
        private readonly CartService _cartService;

        public ProductListModel(GlassesStoreDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public List<Product> Products { get; set; } = new List<Product>();

        [BindProperty]
        public int ProductId { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public IActionResult OnGet()
        {
            // 🔐 Check login
            if (HttpContext.Session.GetString("USERNAME") == null)
            {
                return RedirectToPage("/Login");
            }

            // 🔐 Check role
            var role = HttpContext.Session.GetString("ROLE");
            if (role != "Customer")
            {
                return RedirectToPage("/Login");
            }

            // ✅ Chỉ lấy sản phẩm còn bán
            Products = _context.Products
                .Where(p => !p.IsDeleted && p.Stock > 0)
                .ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("USERNAME") == null)
                return RedirectToPage("/Login");

            var role = HttpContext.Session.GetString("ROLE");
            if (role != "Customer")
                return RedirectToPage("/Login");

            var product = _context.Products.Find(ProductId);

            // ✅ Check tồn kho
            if (product != null && !product.IsDeleted && product.Stock >= Quantity)
            {
                _cartService.AddToCart(product, Quantity);
            }

            return RedirectToPage();
        }

        // ➖ Giảm số lượng
        public IActionResult OnPostDecrease(int removeId)
        {
            if (HttpContext.Session.GetString("USERNAME") == null)
                return RedirectToPage("/Login");

            _cartService.DecreaseQuantity(removeId);
            return RedirectToPage();
        }

        // ❌ Xóa sản phẩm khỏi giỏ
        public IActionResult OnPostRemoveAll(int removeId)
        {
            if (HttpContext.Session.GetString("USERNAME") == null)
                return RedirectToPage("/Login");

            _cartService.RemoveItem(removeId);
            return RedirectToPage();
        }
    }
}