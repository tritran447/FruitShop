using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.UserOtps
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly FruitShopContext _context;

        public UserOtpRepository(FruitShopContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserOtp otp)
        {
            await _context.UserOtps.AddAsync(otp);
        }

        public void Update(UserOtp otp)
        {
            _context.UserOtps.Update(otp);
        }

        public async Task<UserOtp?> GetLatestOtpAsync(int customerId, string purpose)
        {
            return await _context.UserOtps
                .Where(o => o.CustomerID == customerId && o.Purpose == purpose)
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
