using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }

        /// <summary>
        /// When the order was placed (UTC).
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Total amount paid.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// E.g. “Pending”, “Shipped”, “Delivered”, etc.
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// Shipping address used for this order.
        /// </summary>
        public string ShipAddress { get; set; } = null!;

        /// <summary>
        /// The individual items in this order.
        /// </summary>
        public List<OrderHistoryItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// One line-item in an order.
    /// </summary>
    public class OrderHistoryItemDto
    {
        /// <summary>
        /// The product that was ordered.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Name of the product at time of purchase.
        /// </summary>
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Quantity purchased.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price at time of purchase.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Line total: Quantity × Price.
        /// </summary>
        public decimal LineTotal => Quantity * Price;
    }

}
