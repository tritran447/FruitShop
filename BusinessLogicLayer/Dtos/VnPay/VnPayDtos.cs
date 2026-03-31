namespace BusinessLogicLayer.Dtos.VnPay
{
    public class VnPaySettings
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string CurrCode { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
    }

    public class VnPayRequestDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VnPayResponseDto
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; } = "VnPay";
        public string OrderDescription { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string VnPayResponseCode { get; set; } = string.Empty;
    }
}
