using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FruitShop.client.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public List<CustomerDto> Customers { get; set; } = new();

        [BindProperty]
        public CustomerDto CreateCustomer { get; set; } = new();

        [BindProperty]
        public CustomerDto EditCustomer { get; set; } = new();

        [BindProperty]
        public int DeleteId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var pre = EnsureAdmin();
            if (pre is IActionResult redirect) return redirect;

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var pre = EnsureAdmin();
            if (pre is IActionResult redirect) return redirect;

            var client = PrepareClient();
            var resp = await client.PostAsJsonAsync("api/Customer", CreateCustomer);
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Create failed");

            CreateCustomer = new();
            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var pre = EnsureAdmin();
            if (pre is IActionResult redirect) return redirect;

            var client = PrepareClient();
            var resp = await client.PutAsJsonAsync(
                $"api/Customer/{EditCustomer.CustomerId}", EditCustomer);
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Update failed");

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var pre = EnsureAdmin();
            if (pre is IActionResult redirect) return redirect;

            var client = PrepareClient();
            var resp = await client.DeleteAsync($"api/Customer/{DeleteId}");
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Delete failed");

            await LoadAsync();
            return Page();
        }

        // common load-list
        private async Task LoadAsync()
        {
            var client = PrepareClient();
            var resp = await client.GetAsync("api/Customer");
            Customers = resp.IsSuccessStatusCode
                ? (await resp.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>())?.ToList()
                  ?? new()
                : new();
        }

        // helper to attach JWT
        private HttpClient PrepareClient()
        {
            var token = HttpContext.Session.GetString("JwtToken")!;
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // Redirect if not logged in or not admin
        private IActionResult? EnsureAdmin()
        {
            // 1) must have a token
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Authen/Login_Register");

            // 2) get user data
            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToPage("/Authen/Login_Register");

            var user = JsonSerializer.Deserialize<CustomerDto>(userJson);
            if (user == null ||
                !user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Authen/AccessDenied");
            }

            return null; // all good
        }
    }
}
