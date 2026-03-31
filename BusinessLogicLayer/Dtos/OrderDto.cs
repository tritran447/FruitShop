using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class OrderDto
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShipAddress { get; set; } = string.Empty;

        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
