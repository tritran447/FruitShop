using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Admin.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        private List<OrderDto> _allOrders = new();
        public List<OrderDto> PagedOrders { get; set; } = new();

        public Dictionary<int, string> CustomerNames { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [BindProperty] public int EditOrderId { get; set; }
        [BindProperty] public string NewStatus { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            await LoadAllAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            var client = PrepareClient();

            // Fetch the existing order to retain OrderDetails and recalculate correct TotalAmount
            var getResp = await client.GetAsync($"api/Order/{EditOrderId}");
            if (!getResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Order not found or could not verify.");
                await LoadAllAsync();
                ApplyPaging();
                return Page();
            }

            var existingOrder = await getResp.Content.ReadFromJsonAsync<OrderDto>();
            if (existingOrder == null)
            {
                ModelState.AddModelError("", "Deserialization failed for existing order.");
                await LoadAllAsync();
                ApplyPaging();
                return Page();
            }

            existingOrder.Status = NewStatus;

            var putResp = await client.PutAsJsonAsync($"api/Order/{EditOrderId}", existingOrder);
            if (!putResp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Failed to update order status.");

            await LoadAllAsync();
            ApplyPaging();
            return Page();
        }

        private async Task LoadAllAsync()
        {
            var client = PrepareClient();

            // Load Orders
            var resp = await client.GetAsync("api/Order");
            if (resp.IsSuccessStatusCode)
                _allOrders = await resp.Content.ReadFromJsonAsync<List<OrderDto>>() ?? new();
            else
                _allOrders = new();

            // Sort by descending date 
            _allOrders = _allOrders.OrderByDescending(o => o.OrderDate).ToList();

            // Load Customers to map ID to Name
            var custResp = await client.GetAsync("api/Customer");
            if (custResp.IsSuccessStatusCode)
            {
                var customers = await custResp.Content.ReadFromJsonAsync<List<CustomerDto>>();
                if (customers != null)
                {
                    CustomerNames = customers.ToDictionary(c => c.CustomerId, c => c.FullName);
                }
            }
        }

        private void ApplyPaging()
        {
            TotalCount = _allOrders.Count;
            PagedOrders = _allOrders
              .Skip((PageIndex - 1) * PageSize)
              .Take(PageSize)
              .ToList();
        }

        private HttpClient PrepareClient()
        {
            var token = HttpContext.Session.GetString("JwtToken")!;
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private IActionResult? EnsureAdmin()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Authen/Login_Register");

            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson)) return RedirectToPage("/Authen/Login_Register");

            try
            {
                var user = JsonSerializer.Deserialize<CustomerDto>(userJson);
                if (user == null || !user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    return RedirectToPage("/Authen/AccessDenied");
            }
            catch
            {
                return RedirectToPage("/Authen/Login_Register");
            }

            return null;
        }
    }
}
