using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class RevenueDto
    {
        public decimal Today { get; set; }
        public decimal ThisMonth { get; set; }
        public decimal ThisYear { get; set; }
    }

}
