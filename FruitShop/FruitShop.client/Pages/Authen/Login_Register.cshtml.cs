using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FruitShop.client.Pages.Authen
{
    public class Login_RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Login_RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginDto LoginModel { get; set; } = new();

        [BindProperty]
        public RegisterDto RegisterModel { get; set; } = new();

        public string? LoginMessage { get; set; }
        public bool LoginSuccess { get; set; }

        public string? RegisterMessage { get; set; }
        public bool RegisterSuccess { get; set; }

        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/Authen/Login", LoginModel);

            var apiResp = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();

            if (apiResp != null && apiResp.Success)
            {
                HttpContext.Session.SetString("JwtToken", apiResp.Token ?? "");
                if (apiResp.Data != null)
                {
                    var jsonData = JsonSerializer.Serialize(apiResp.Data);
                    HttpContext.Session.SetString("UserData", jsonData);
                }

                var user = apiResp.Data!;
                if (user.Role != null && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToPage("/Admin/Dashboard/Index");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                if (apiResp?.Message?.ToLower().Contains("not verified") == true)
                {
                    TempData["VerifyPrompt"] = $"The email {LoginModel.Email} has not been verified. Please check your inbox for the OTP code.";
                    return RedirectToPage("/Authen/Verify_Email", new { email = LoginModel.Email });
                }

                LoginSuccess = false;
                LoginMessage = apiResp?.Message ?? "Login failed.";
            }

            return Page();
        }


        public async Task<IActionResult> OnPostRegisterAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/Authen/Register", RegisterModel);

            var apiResp = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();

            if (apiResp != null && apiResp.Success)
            {
                return RedirectToPage("/Authen/Verify_Email", new
                {
                    email = RegisterModel.Email
                });
            }
            else
            {
                RegisterSuccess = false;
                RegisterMessage = apiResp?.Message ?? "Registration failed.";
                return Page();
            }
        }
    }
}
