using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class UserOtp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OtpId { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        [MaxLength(10)]
        public string OtpCode { get; set; } = null!;

        [Required]
        public DateTime ExpirationTime { get; set; }

        public bool IsUsed { get; set; } = false;

        [MaxLength(50)]
        public string? Purpose { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public Customer Customer { get; set; } = null!;
    }
}
