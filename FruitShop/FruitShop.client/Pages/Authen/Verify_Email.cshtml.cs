using AutoMapper;
using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FruitShop.client.Pages.Authen
{
    public class Verify_EmailModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Verify_EmailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string OtpCode { get; set; } = string.Empty;

        public string? Message { get; set; }
        public bool Success { get; set; }

        public IActionResult OnGet(string? email = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                Email = email;
            }

            var jsonData = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(jsonData))
            {
                var user = JsonSerializer.Deserialize<CustomerDto>(jsonData);
                if (user != null)
                {
                    if (user.IsVerified)
                    {
                        return RedirectToPage("/Index");
                    }

                    if (string.IsNullOrEmpty(Email))
                    {
                        Email = user.Email;
                    }
                }
            }

            if (TempData["VerifyPrompt"] != null)
            {
                Message = TempData["VerifyPrompt"]?.ToString();
                Success = false;
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var dto = new EmailVerificationDto
            {
                Email = Email,
                OtpCode = OtpCode
            };

            var response = await client.PostAsJsonAsync("api/Authen/verify-email", dto);

            var apiResp = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResp != null && apiResp.Success)
            {
                Success = true;
                TempData["VerifySuccess"] = "Your email has been verified successfully. You can now log in.";
                return RedirectToPage("/Authen/Login_Register");
            }
            else
            {
                Success = false;
                Message = apiResp?.Message ?? "Verification failed.";
            }

            return Page();
        }
    }

}
