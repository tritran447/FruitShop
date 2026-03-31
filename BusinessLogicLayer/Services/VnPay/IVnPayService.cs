using BusinessLogicLayer.Dtos.VnPay;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequestDto model);
        VnPayResponseDto PaymentExecute(IQueryCollection collections);
        Task<string> QueryTransactionAsync(string orderCode, string createDate);
        Task<string> RefundAsync(VnPayRequestDto model, string createBy);
    }
}
