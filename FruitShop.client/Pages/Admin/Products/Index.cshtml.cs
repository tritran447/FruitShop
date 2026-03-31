using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // 1) All products fetched from API
        private List<ProductDto> _allProducts = new();

        // 2) Paged view
        public List<ProductDto> PagedProducts { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();

        // 3) Bind page index & size
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // 4) Create / Edit / Delete bindings
        [BindProperty] public ProductDto CreateProduct { get; set; } = new();
        [BindProperty] public ProductDto EditProduct { get; set; } = new();
        [BindProperty] public int DeleteId { get; set; }

        [BindProperty] public IFormFile? CreateImageFile { get; set; }
        [BindProperty] public IFormFile? EditImageFile { get; set; }

        private readonly IWebHostEnvironment _env;

        public IndexModel(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // admin check
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            await LoadAllAsync();
            await LoadCategoriesAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            if (CreateImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CreateImageFile.FileName);
                var savePath = Path.Combine(_env.WebRootPath, "assets", "images", "product-fruit", fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await CreateImageFile.CopyToAsync(stream);
                }
                CreateProduct.Image = fileName;
            }

            var client = PrepareClient();
            var resp = await client.PostAsJsonAsync("api/Product", CreateProduct);
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Create failed");

            CreateProduct = new();
            await LoadAllAsync();
            await LoadCategoriesAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            if (EditImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(EditImageFile.FileName);
                var savePath = Path.Combine(_env.WebRootPath, "assets", "images", "product-fruit", fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await EditImageFile.CopyToAsync(stream);
                }
                EditProduct.Image = fileName;
            }

            var client = PrepareClient();
            var resp = await client.PutAsJsonAsync(
                $"api/Product/{EditProduct.ProductId}", EditProduct);
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Update failed");

            await LoadAllAsync();
            await LoadCategoriesAsync();
            ApplyPaging();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var chk = EnsureAdmin();
            if (chk is IActionResult r) return r;

            var client = PrepareClient();
            var resp = await client.DeleteAsync($"api/Product/{DeleteId}");
            if (!resp.IsSuccessStatusCode)
                ModelState.AddModelError("", "Delete failed");

            await LoadAllAsync();
            await LoadCategoriesAsync();
            // if current page now empty, go back one page
            if ((PageIndex - 1) * PageSize >= _allProducts.Count && PageIndex > 1)
                PageIndex--;
            ApplyPaging();
            return Page();
        }

        private async Task LoadAllAsync()
        {
            var client = PrepareClient();
            var resp = await client.GetAsync("api/Product");
            if (resp.IsSuccessStatusCode)
                _allProducts = await resp.Content.ReadFromJsonAsync<List<ProductDto>>()
                               ?? new();
            else
                _allProducts = new();
        }

        private async Task LoadCategoriesAsync()
        {
            var client = PrepareClient();
            var resp = await client.GetAsync("api/Categories");
            if (resp.IsSuccessStatusCode)
                Categories = await resp.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
            else
                Categories = new();
        }

        private void ApplyPaging()
        {
            TotalCount = _allProducts.Count;
            PagedProducts = _allProducts
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
