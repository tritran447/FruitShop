using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FruitShop.client.Pages.Shop
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // New: nhận search từ querystring
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        // Filters & pagination
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 12;

        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; } = "default";

        [BindProperty(SupportsGet = true)]
        public int? CategoryFilter { get; set; }

        // Data for rendering
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ProductDto> PopularProducts { get; set; } = new();
        public List<ProductDto> AllProducts { get; set; } = new();
        public List<ProductDto> PagedProducts { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // (1) ALWAYS LOAD CATEGORY LIST
            var cats = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Categories");
            Categories = cats?.ToList() ?? new();

            // (2) ALWAYS LOAD POPULAR PRODUCTS
            var pops = await client.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Product/popular?count=3");
            PopularProducts = pops?.ToList() ?? new();

            // (3) ALWAYS LOAD ALL PRODUCTS (for category counts)
            var all = await client.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Product");
            AllProducts = all?.ToList() ?? new();

            // (4) HANDLE SEARCH CASE
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var url = $"api/Product/search?name={Uri.EscapeDataString(Search)}&pageIndex={PageIndex}&pageSize={PageSize}";

                // Nếu có lọc category
                if (CategoryFilter.HasValue)
                    url += $"&categoryId={CategoryFilter.Value}";

                var result = await client.GetFromJsonAsync<PaginatedList<ProductDto>>(url);

                if (result != null)
                {
                    PagedProducts = result.ToList();
                    TotalCount = result.TotalCount;
                }
                else
                {
                    PagedProducts = new List<ProductDto>();
                    TotalCount = 0;
                }
            }
            else
            {
                // (5) NON-SEARCH CASE: FILTER AND SORT ALL PRODUCTS
                var filtered = AllProducts
                    .Where(p => !CategoryFilter.HasValue || p.CategoryId == CategoryFilter.Value);

                filtered = Sort switch
                {
                    "price_asc" => filtered.OrderBy(p => p.Price),
                    "price_desc" => filtered.OrderByDescending(p => p.Price),
                    _ => filtered
                };

                TotalCount = filtered.Count();
                PagedProducts = filtered
                    .Skip((PageIndex - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }
    }
}
