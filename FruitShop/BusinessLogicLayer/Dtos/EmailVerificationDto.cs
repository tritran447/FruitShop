using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class EmailVerificationDto
    {
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
    }
}
