using BusinessLogicLayer.Dtos;

namespace BusinessLogicLayer.Services.Authen
{
    public interface IAuthService
    {
        Task<ApiResponse<CustomerDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<CustomerDto>> LoginAsync(LoginDto dto);

        Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto dto, int customerId);

        Task<ApiResponse<object>> VerifyEmailAsync(string email, string otpCode);

        Task<ApiResponse<object>> RequestPasswordResetAsync(string email);
    }
}
