using System.Text.Json;
using BusinessLogicLayer.Dtos;
using FruitShop.client.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public string CartData { get; set; } = "";
        [BindProperty]
        public string ShipAddress { get; set; } = "";

        public OrderDto? CreatedOrder { get; private set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1) Deserialize cart JSON
            if (string.IsNullOrEmpty(CartData))
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return Page();
            }

            List<CartItemDto>? cartItems;
            try
            {
                cartItems = JsonSerializer.Deserialize<List<CartItemDto>>(CartData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                ModelState.AddModelError("", "Invalid cart data.");
                return Page();
            }

            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return Page();
            }

            // 2) Validate shipping address
            if (string.IsNullOrWhiteSpace(ShipAddress))
            {
                ModelState.AddModelError(nameof(ShipAddress), "Please enter a shipping address.");
                return Page();
            }

            // 3) Get logged-in customer from session
            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToPage("/Authen/Login_Register");
            var customer = JsonSerializer.Deserialize<CustomerDto>(userJson)!;

            // 4) Build CreateOrderDto
            var createDto = new CreateOrderDto
            {
                CustomerId = customer.CustomerId,
                ShipAddress = ShipAddress,
                Status = "Pending",
                Details = cartItems.Select(ci => new CreateOrderDetailDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Price ?? 0m
                }).ToList()
            };

            // 5) Call API tạo đơn
            var client = _httpClientFactory.CreateClient("ApiClient");
            var resp = await client.PostAsJsonAsync("/api/Order", createDto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to place order: {error}");
                return Page();
            }

            // 6) Get Created Order details
            CreatedOrder = await resp.Content.ReadFromJsonAsync<OrderDto>();

            // Stay on page to show VNPAY Modal
            return Page();
        }
    }
}
