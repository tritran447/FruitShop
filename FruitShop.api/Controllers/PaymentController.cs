using BusinessLogicLayer.Dtos.VnPay;
using BusinessLogicLayer.Services.VnPay;
using DataAccessLayer.Repositories.Orders;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FruitShop.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IOrderRepository _orderRepository;

        public PaymentController(IVnPayService vnPayService, IOrderRepository orderRepository)
        {
            _vnPayService = vnPayService;
            _orderRepository = orderRepository;
        }

        [HttpPost("create")]
        public IActionResult CreatePayment(VnPayRequestDto model)
        {
            var url = _vnPayService.CreatePaymentUrl(HttpContext, model);
            return Ok(new { paymentUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            
            // Note: In real-world, you might want to redirect the user to a success/fail page on the frontend
            // with the response data.
            
            return Ok(response);
        }

        [HttpGet("vnpay-return")]
        public IActionResult VnPayReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            // Return to frontend with status
            // Example: return Redirect($"http://localhost:5110/payment-result?success={response.Success}&message={response.VnPayResponseCode}");
            return Ok(response);
        }

        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnPayIpn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            
            if (response.Success)
            {
                var order = await _orderRepository.GetByCodeAsync(response.OrderId);
                if (order != null)
                {
                    if (order.Status != "Paid" && order.Status != "Failed")
                    {
                        if (response.VnPayResponseCode == "00")
                        {
                            order.Status = "Paid";
                        }
                        else
                        {
                            order.Status = "Failed";
                        }
                        
                        _orderRepository.Update(order);
                        await _orderRepository.SaveChangesAsync();
                        
                        // VNPAY IPN expects a specific JSON response
                        return Ok(new { RspCode = "00", Message = "Confirm Success" });
                    }
                    else
                    {
                        return Ok(new { RspCode = "02", Message = "Order already confirmed" });
                    }
                }
                else
                {
                    return Ok(new { RspCode = "01", Message = "Order not found" });
                }
            }
            
            return Ok(new { RspCode = "97", Message = "Invalid Checksum" });
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryTransaction(string orderCode, string createDate)
        {
            var result = await _vnPayService.QueryTransactionAsync(orderCode, createDate);
            return Ok(result);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund(VnPayRequestDto model, string createBy)
        {
            var result = await _vnPayService.RefundAsync(model, createBy);
            return Ok(result);
        }
    }
}
