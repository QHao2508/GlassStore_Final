using GlassStore.Entities;
using System.Text.Json;
using GlassStore.Models;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext.Session;

    private const string CART_KEY = "cart";

    // 🛒 Get Cart
    public List<CartItem> GetCart()
    {
        var data = Session.GetString(CART_KEY);

        if (string.IsNullOrEmpty(data))
            return new List<CartItem>();

        return JsonSerializer.Deserialize<List<CartItem>>(data);
    }

    // ➕ Add to Cart
    public void AddToCart(Product product, int quantity)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(x => x.ProductId == product.ProductId);

        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            });
        }

        SaveCart(cart);
    }

    // ➖ HÀM MỚI: Decrease Quantity (Giảm số lượng)
    public void DecreaseQuantity(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);

        if (item != null)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--; // Trừ đi 1
            }
            else
            {
                cart.Remove(item); // Nếu = 1 mà trừ thì xoá luôn
            }
            SaveCart(cart); // LƯU LẠI VÀO SESSION (QUAN TRỌNG NHẤT)
        }
    }

    // ❌ Remove
    public void RemoveItem(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(x => x.ProductId == productId);
        SaveCart(cart); // LƯU LẠI VÀO SESSION
    }

    // 💾 Save
    private void SaveCart(List<CartItem> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        Session.SetString(CART_KEY, json);
    }

    // 💰 Total
    public decimal GetTotal()
    {
        return GetCart().Sum(x => x.Price * x.Quantity);
    }

    // 🧹 Clear
    public void ClearCart()
    {
        Session.Remove(CART_KEY);
    }
}