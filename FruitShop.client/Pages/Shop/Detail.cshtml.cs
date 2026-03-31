using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FruitShop.client.Pages.Shop
{
    public class DetailModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DetailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // BindProperty để dùng trong Razor
        public ProductDto Product { get; set; }
        public List<ProductDto> RelatedProducts { get; set; } = new();

        // route: /Shop/Detail?id=123
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // 1. Lấy chi tiết sản phẩm
            var resp = await client.GetAsync($"api/Product/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            Product = await resp.Content.ReadFromJsonAsync<ProductDto>()
                      ?? throw new ApplicationException("Cannot deserialize ProductDto");

            // 2. Lấy sản phẩm liên quan (4 cái)
            var rel = await client.GetFromJsonAsync<IEnumerable<ProductDto>>(
                        $"api/Product/related?productId={id}&count=4");
            if (rel != null)
                RelatedProducts = rel.ToList();

            return Page();
        }
    }
}
