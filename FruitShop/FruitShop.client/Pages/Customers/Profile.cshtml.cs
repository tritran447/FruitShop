using System.Net.Http.Headers;
using System.Text.Json;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Customers
{
    public class ProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CustomerDto Profile { get; set; } = new();

        public string? Message { get; set; }
        public bool Success { get; set; }

        [BindProperty]
        public ChangePasswordDto Pwd { get; set; } = new();

        public string? PwdMessage { get; set; }
        public bool PwdSuccess { get; set; }

        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Authen/Login_Register");

            var jsonData = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(jsonData))
                return RedirectToPage("/Authen/Login_Register");

            Profile = JsonSerializer.Deserialize<CustomerDto>(jsonData)!;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JwtToken")!;
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PutAsJsonAsync(
                $"api/Customer/{Profile.CustomerId}", Profile);

            if (resp.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString(
                    "UserData", JsonSerializer.Serialize(Profile));
                Success = true;
                Message = "Profile updated successfully.";
            }
            else
            {
                Success = false;
                var apiResp = await resp.Content
                    .ReadFromJsonAsync<ApiResponse<CustomerDto>>();
                Message = apiResp?.Message ?? "Update failed.";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var token = HttpContext.Session.GetString("JwtToken")!;
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync(
                "api/Authen/change-password", Pwd);

            var apiResp = await resp.Content
                .ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResp != null && apiResp.Success)
            {
                PwdSuccess = true;
                PwdMessage = apiResp.Message ?? "Password changed.";
            }
            else
            {
                PwdSuccess = false;
                PwdMessage = apiResp?.Message ?? "Change failed.";
            }
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Authen/Login_Register");
        }
    } 
}
