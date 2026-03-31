using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace FruitShop.client.Pages
{
    public class VnPayTestModel : PageModel
    {
        public string OrderCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public void OnGet()
        {
            // Normally you would get this from a database or a cart.
            // For testing, we generate a unique code.
            OrderCode = "FRUIT_" + DateTime.Now.Ticks.ToString();
            Amount = 150000; // Example: 150,000 VND
        }
    }
}
