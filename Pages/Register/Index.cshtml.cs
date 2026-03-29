using GlassStore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GlassStore.Pages.Register
{
    public class RegisterModel : PageModel
    {
        private readonly GlassesStoreDbContext _context;
        public RegisterModel(GlassesStoreDbContext context) => _context = context;

        [BindProperty] public string? Username { get; set; }
        [BindProperty] public string? Password { get; set; }
        public string? Message { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Please enter username and password.";
                return Page();
            }

            var exist = _context.Users.FirstOrDefault(u => u.Username == Username);
            if (exist != null)
            {
                Message = "Username already exists!";
                return Page();
            }

            // Khi user tạo account mặc định role = Customer
            var user = new User
            {
                Username = Username,
                Password = Password,
                Role = "Customer"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Redirect về Login
            return RedirectToPage("/Login");
        }
    }
}