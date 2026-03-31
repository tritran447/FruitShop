using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FruitShop.client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // Nếu cần dùng trong code-behind
        public List<ProductDto> PopularProducts { get; set; } = new();
        public List<ProductDto> BestSellerProducts { get; set; } = new();

        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["GivenAPIBaseUrl"];

            var popularUrl = $"{baseUrl}/api/Product/popular?count=5";
            var popularResponse = await client.GetAsync(popularUrl);
            if (popularResponse.IsSuccessStatusCode)
            {
                var popular = await popularResponse.Content.ReadFromJsonAsync<List<ProductDto>>();
                PopularProducts = popular ?? new List<ProductDto>();
            }

            var bestSellerUrl = $"{baseUrl}/api/Product/best-sellers?count=5";
            var bestSellerResponse = await client.GetAsync(bestSellerUrl);
            if (bestSellerResponse.IsSuccessStatusCode)
            {
                var best = await bestSellerResponse.Content.ReadFromJsonAsync<List<ProductDto>>();
                BestSellerProducts = best ?? new List<ProductDto>();
            }

            ViewData["PopularProducts"] = PopularProducts;
            ViewData["BestSellerProducts"] = BestSellerProducts;
        }
    }
}
