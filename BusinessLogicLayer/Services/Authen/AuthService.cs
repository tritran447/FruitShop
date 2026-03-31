using AutoMapper;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Customers;
using DataAccessLayer.Repositories.UserOtps;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogicLayer.Services.Authen
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IEmailService _email;
        private readonly IUserOtpRepository _otpRepo;

        public AuthService(
            ICustomerRepository repo,
            IUserOtpRepository otpRepo,
            IMapper mapper,
            IConfiguration config,
            IEmailService email)
        {
            _repo = repo;
            _otpRepo = otpRepo;
            _mapper = mapper;
            _config = config;
            _email = email;
        }

        public async Task<ApiResponse<CustomerDto>> RegisterAsync(RegisterDto dto)
        {
            var exist = await _repo.GetByEmailAsync(dto.Email);
            if (exist != null)
            {
                return new ApiResponse<CustomerDto>
                {
                    Success = false,
                    Token = string.Empty,
                    Message = "Email is already registered.",
                    Data = null
                };
            }

            var customerEntity = _mapper.Map<Customer>(dto);
            customerEntity.Role = "User";
            customerEntity.IsVerified = false;

            await _repo.AddAsync(customerEntity);
            await _repo.SaveChangesAsync();

            var otpCode = GenerateNumericOtp(6);
            var otp = new UserOtp
            {
                CustomerID = customerEntity.CustomerId,
                OtpCode = otpCode,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Purpose = "EmailVerification"
            };
            await _otpRepo.AddAsync(otp);
            await _otpRepo.SaveChangesAsync();

            await _email.SendAsync(customerEntity.Email, "Your OTP Code", $"Your verification code is: {otpCode}");

            var customerDto = _mapper.Map<CustomerDto>(customerEntity);

            return new ApiResponse<CustomerDto>
            {
                Success = true,
                Token = string.Empty,
                Message = "Registration successful. Please verify your email using the OTP sent.",
                Data = customerDto
            };
        }

        public async Task<ApiResponse<object>> VerifyEmailAsync(string email, string otpCode)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
                return new ApiResponse<object> { Success = false, Message = "User not found." };

            if (user.IsVerified)
                return new ApiResponse<object> { Success = false, Message = "Email already verified." };

            var otp = await _otpRepo.GetLatestOtpAsync(user.CustomerId, "EmailVerification");

            if (otp == null || otp.IsUsed || otp.ExpirationTime < DateTime.UtcNow || otp.OtpCode != otpCode)
            {
                return new ApiResponse<object> { Success = false, Message = "Invalid or expired OTP." };
            }

            otp.IsUsed = true;
            user.IsVerified = true;

            _otpRepo.Update(otp);
            _repo.Update(user);

            await _otpRepo.SaveChangesAsync();
            await _repo.SaveChangesAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Email verified successfully."
            };
        }


        public async Task<ApiResponse<CustomerDto>> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null || user.Password != dto.Password)
            {
                return new ApiResponse<CustomerDto>
                {
                    Success = false,
                    Token = string.Empty,
                    Message = "Invalid email or password.",
                    Data = null
                };
            }

            if (!user.IsVerified)
            {
                return new ApiResponse<CustomerDto>
                {
                    Success = false,
                    Token = string.Empty,
                    Message = "Your email is not verified. Please check your inbox and enter the OTP code.",
                    Data = null
                };
            }

            var jwtSection = _config.GetSection("Jwt");
            var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiryDays = int.Parse(jwtSection["ExpiryInDays"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.CustomerId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(expiryDays),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwtToken);

            var customerDto = new CustomerDto
            {
                CustomerId = user.CustomerId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };

            return new ApiResponse<CustomerDto>
            {
                Success = true,
                Token = token,
                Message = "Login successful.",
                Data = customerDto
            };
        }


        public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto dto, int customerId)
        {
            var user = await _repo.GetByIdAsync(customerId);
            if (user == null)
                return new ApiResponse<object> { Success = false, Message = "User not found." };

            if (user.Password != dto.OldPassword)
                return new ApiResponse<object> { Success = false, Message = "Current password is incorrect." };

            if (dto.NewPassword != dto.ConfirmPassword)
                return new ApiResponse<object> { Success = false, Message = "New passwords do not match." };

            user.Password = dto.NewPassword;
            _repo.Update(user);
            await _repo.SaveChangesAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Password changed successfully."
            };
        }

        public async Task<ApiResponse<object>> RequestPasswordResetAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email not found."
                };

            // 1. Sinh mã OTP
            var otpCode = GenerateNumericOtp(6);

            // 2. Sinh mật khẩu tạm thời
            var tempPassword = GenerateRandomPassword(10);
            user.Password = tempPassword;

            // 3. Lưu OTP
            var otp = new UserOtp
            {
                CustomerID = user.CustomerId,
                OtpCode = otpCode,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Purpose = "PasswordReset"
            };

            await _otpRepo.AddAsync(otp);
            _repo.Update(user);

            await _otpRepo.SaveChangesAsync();
            await _repo.SaveChangesAsync();

            await _email.SendAsync(user.Email,
                "Password Reset Code & Temporary Password",
                $"Your password reset code is: {otpCode}\nYour temporary password is: {tempPassword}");

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Password reset code and temporary password have been sent to your email."
            };
        }


        private string GenerateNumericOtp(int length = 6)
        {
            var random = new Random();
            string otp = "";
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10); // từ 0 đến 9
            }
            return otp;
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }





    }
}
