using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FruitShop.client.Pages.Authen
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ForgotPasswordModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public string? Message { get; set; }
        public bool Success { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Please enter your email.";
                Success = false;
                return Page();
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/Authen/request-password-reset", new { Email });

            if (response.IsSuccessStatusCode)
            {
                Success = true;
                Message = "Password reset have been sent to your email.";
            }
            else
            {
                Success = false;
                Message = "Email not found or failed to send reset link.";
            }

            return Page();
        }
    }
}
