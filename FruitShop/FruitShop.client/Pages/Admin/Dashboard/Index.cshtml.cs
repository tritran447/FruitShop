using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FruitShop.client.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ProductSaleDto> TopProducts { get; set; } = new();
        public RevenueDto Revenue { get; set; } = new();
        public int ActiveCustomerCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Check quyền admin
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            var client = PrepareClient();

            // 2. Gọi API
            TopProducts = await client.GetFromJsonAsync<List<ProductSaleDto>>("api/Admin/top-selling") ?? new();
            Revenue = await client.GetFromJsonAsync<RevenueDto>("api/Admin/revenue") ?? new();
            ActiveCustomerCount = await client.GetFromJsonAsync<int>("api/Admin/active-customers");

            return Page();
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
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Authen/Login_Register");

            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToPage("/Authen/Login_Register");

            var user = JsonSerializer.Deserialize<CustomerDto>(userJson);
            if (user == null ||
                !user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToPage("/Authen/AccessDenied");

            return null;
        }
    }
}
