using BusinessLogicLayer.Dtos.VnPay;
using BusinessLogicLayer.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace BusinessLogicLayer.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public VnPayService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public string CreatePaymentUrl(HttpContext context, VnPayRequestDto model)
        {
            var vnpay = new VnPayLibrary();
            
            var tmnCode = _configuration["VnPay:TmnCode"]?.Trim() ?? "";
            var hashSecret = _configuration["VnPay:HashSecret"]?.Trim() ?? "";
            var baseUrl = _configuration["VnPay:BaseUrl"]?.Trim() ?? "";
            var returnUrl = _configuration["VnPay:ReturnUrl"]?.Trim() ?? "";

            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"] ?? "2.1.0");
            vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"] ?? "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());
            
            // MUST use current time, not order creation time — otherwise VNPAY returns code=15 (timeout)
            var now = DateTime.Now;
            vnpay.AddRequestData("vnp_CreateDate", now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"] ?? "VND");
            
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ipAddress == "::1") ipAddress = "127.0.0.1";
            
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"] ?? "vn");
            // VNPAY requires: Vietnamese without diacritics, no special characters
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {model.OrderCode}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", model.OrderCode);
            
            // Expire date = 15 minutes from NOW
            vnpay.AddRequestData("vnp_ExpireDate", now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            return vnpay.CreateRequestUrl(baseUrl, hashSecret);
        }

        public VnPayResponseDto PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderCode = vnpay.GetResponseData("vnp_TxnRef");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash.ToString(), _configuration["VnPay:HashSecret"]?.Trim() ?? "");
            if (!checkSignature)
            {
                return new VnPayResponseDto { Success = false };
            }

            return new VnPayResponseDto
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_OrderCode,
                TransactionId = vnp_TransactionId,
                VnPayResponseCode = vnp_ResponseCode
            };
        }

        public async Task<string> QueryTransactionAsync(string orderCode, string createDate)
        {
            var vnp_TmnCode = _configuration["VnPay:TmnCode"]?.Trim();
            var vnp_HashSecret = _configuration["VnPay:HashSecret"]?.Trim();
            var vnp_ApiUrl = _configuration["VnPay:ApiUrl"]?.Trim();

            var vnp_RequestId = DateTime.Now.Ticks.ToString();
            var vnp_Version = "2.1.0";
            var vnp_Command = "querydr";
            var vnp_TxnRef = orderCode;
            var vnp_OrderInfo = "Truy van don hang " + orderCode;
            var vnp_TransactionDate = createDate;
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_IpAddr = "127.0.0.1";

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TxnRef + "|" + vnp_TransactionDate + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = HmacSHA512(vnp_HashSecret ?? "", signData);

            var queryData = new
            {
                vnp_RequestId,
                vnp_Version,
                vnp_Command,
                vnp_TmnCode,
                vnp_TxnRef,
                vnp_OrderInfo,
                vnp_TransactionDate,
                vnp_CreateDate,
                vnp_IpAddr,
                vnp_SecureHash
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(vnp_ApiUrl, queryData);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RefundAsync(VnPayRequestDto model, string createBy)
        {
            var vnp_TmnCode = _configuration["VnPay:TmnCode"]?.Trim();
            var vnp_HashSecret = _configuration["VnPay:HashSecret"]?.Trim();
            var vnp_ApiUrl = _configuration["VnPay:ApiUrl"]?.Trim();

            var vnp_RequestId = DateTime.Now.Ticks.ToString();
            var vnp_Version = "2.1.0";
            var vnp_Command = "refund";
            var vnp_TransactionType = "02"; // 02: Hoan tra toan phan
            var vnp_TxnRef = model.OrderCode;
            var vnp_Amount = ((long)(model.Amount * 100)).ToString();
            var vnp_OrderInfo = "Hoan tien don hang " + model.OrderCode;
            var vnp_TransactionDate = model.CreatedDate.ToString("yyyyMMddHHmmss");
            var vnp_CreateBy = createBy;
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_IpAddr = "127.0.0.1";

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = HmacSHA512(vnp_HashSecret ?? "", signData);

            var refundData = new
            {
                vnp_RequestId,
                vnp_Version,
                vnp_Command,
                vnp_TmnCode,
                vnp_TransactionType,
                vnp_TxnRef,
                vnp_Amount,
                vnp_OrderInfo,
                vnp_TransactionDate,
                vnp_CreateBy,
                vnp_CreateDate,
                vnp_IpAddr,
                vnp_SecureHash
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(vnp_ApiUrl, refundData);
            return await response.Content.ReadAsStringAsync();
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}
