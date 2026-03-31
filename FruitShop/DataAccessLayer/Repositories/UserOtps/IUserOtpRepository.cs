using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.UserOtps
{
    public interface IUserOtpRepository
    {
        Task AddAsync(UserOtp otp);
        void Update(UserOtp otp);
        Task<UserOtp?> GetLatestOtpAsync(int customerId, string purpose);
        Task<int> SaveChangesAsync();
    }
}
