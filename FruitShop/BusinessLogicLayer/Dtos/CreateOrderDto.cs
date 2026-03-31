using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public string ShipAddress { get; set; } = "";
        public string Status { get; set; } = "pending";
        public List<CreateOrderDetailDto> Details { get; set; } = new();
    }
}
