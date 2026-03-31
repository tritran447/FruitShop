using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? QuantityInStock { get; set; }

    public int? CategoryId { get; set; }

    public string? Image { get; set; }

    public bool? Status { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
