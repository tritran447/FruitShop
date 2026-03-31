using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public bool IsVerified { get; set; } = false;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
}
