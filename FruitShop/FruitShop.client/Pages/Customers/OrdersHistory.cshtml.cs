using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Customers
{
    public class OrdersHistoryModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OrdersHistoryModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public List<OrderHistoryDto> History { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // 1) Auth check
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Authen/Login_Register");

            // 2) Get current user
            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToPage("/Authen/Login_Register");

            var user = JsonSerializer.Deserialize<CustomerDto>(userJson);
            if (user == null)
                return RedirectToPage("/Authen/Login_Register");

            // 3) Call history API
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client
                .GetFromJsonAsync<IEnumerable<OrderHistoryDto>>(
                    $"api/Order/history/{user.CustomerId}");

            History = resp?.OrderByDescending(o => o.OrderDate).ToList()
                      ?? new List<OrderHistoryDto>();

            return Page();
        }
    }
}
