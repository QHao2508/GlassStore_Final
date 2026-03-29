using GlassStore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace GlassStore.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;

        public LoginModel(GlassesStoreDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? Message { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users
                .FirstOrDefault(x =>
                    x.Username == Username &&
                    x.Password == Password);

            if (user == null)
            {
                Message = "Tài khoản hoặc mật khẩu không hợp lệ";
                return Page();
            }

            HttpContext.Session.SetInt32("USER_ID", user.UserId);
            HttpContext.Session.SetString("USERNAME", user.Username);
            HttpContext.Session.SetString("ROLE", user.Role);

            if (user.Role == "Admin")
            {
                return RedirectToPage("/Admin/Index");
            }

            return RedirectToPage("/Customer/ProductList");
        }
    }
}