using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? QuantityInStock { get; set; }
        public int? CategoryId { get; set; }
        public string? Image { get; set; }
        public bool? Status { get; set; }
        public CategoryDto? Category { get; set; }
    }
}
