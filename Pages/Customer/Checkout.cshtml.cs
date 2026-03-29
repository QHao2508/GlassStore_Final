using GlassStore.Entities;
using GlassStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GlassStore.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;
        private readonly CartService _cartService;

        public CheckoutModel(GlassesStoreDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [BindProperty]
        public CheckoutForm Form { get; set; } = new CheckoutForm();

        public List<CartItem> CartItems { get; set; } = new();

        // ⭐ map product từ DB
        public Dictionary<int, Product> Products { get; set; } = new();

        public decimal Total { get; set; }

        public async Task OnGet()
        {
            CartItems = _cartService.GetCart();
            Total = _cartService.GetTotal();

            var ids = CartItems.Select(x => x.ProductId).ToList();

            Products = await _context.Products
                .Where(p => ids.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cart = _cartService.GetCart();

            if (cart == null || cart.Count == 0)
            {
                return RedirectToPage("/Customer/ProductList");
            }

            var userId = HttpContext.Session.GetInt32("USER_ID");

            if (userId == null)
            {
                return RedirectToPage("/Login/Login");
            }

            var order = new Order
            {
                UserId = userId.Value,
                TotalAmount = _cartService.GetTotal()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var groupedItems = cart
                .GroupBy(item => item.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Price = g.First().Price
                });

            foreach (var item in groupedItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                if (product == null) continue;

                product.Stock -= item.Quantity;

                _context.Products.Update(product);

                var detail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                _context.OrderDetails.Add(detail);
            }

            await _context.SaveChangesAsync();

            _cartService.ClearCart();

            return RedirectToPage("/Customer/CheckoutSuccess");
        }
        public class CheckoutForm
        {
            [Required] public string CustomerName { get; set; } = "";
            [Required] public string Phone { get; set; } = "";
            [Required] public string Address { get; set; } = "";
        }
    }
}