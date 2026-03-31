using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        private List<CategoryDto> _allCategories = new();

        public List<CategoryDto> PagedCategories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [BindProperty] public CategoryDto CreateCategory { get; set; } = new();
        [BindProperty] public CategoryDto EditCategory { get; set; } = new();
        [BindProperty] public int DeleteId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            await LoadAllAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            var client = PrepareClient();
            var resp = await client.PostAsJsonAsync("api/Categories", CreateCategory);
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Create failed");

            CreateCategory = new();
            await LoadAllAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            var client = PrepareClient();
            var resp = await client.PutAsJsonAsync(
                $"api/Categories/{EditCategory.CategoryId}", EditCategory);
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Update failed");

            await LoadAllAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            var client = PrepareClient();
            var resp = await client.DeleteAsync($"api/Categories/{DeleteId}");
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Delete failed. Ensure there are no products tied to this category.");

            await LoadAllAsync();
            if ((PageIndex - 1) * PageSize >= _allCategories.Count && PageIndex > 1)
                PageIndex--;
            ApplyPaging();
            return Page();
        }

        private async Task LoadAllAsync()
        {
            var client = PrepareClient();
            var resp = await client.GetAsync("api/Categories");
            if (resp.IsSuccessStatusCode)
                _allCategories = await resp.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
            else
                _allCategories = new();
        }

        private void ApplyPaging()
        {
            TotalCount = _allCategories.Count;
            PagedCategories = _allCategories
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
