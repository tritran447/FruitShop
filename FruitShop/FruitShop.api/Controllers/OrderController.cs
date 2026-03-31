using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FruitShop.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
            => _orderService = orderService;

        /// <summary>
        /// Lấy danh sách tất cả đơn hàng
        /// </summary>
        [HttpGet]
        [EnableQuery] 
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IQueryable<OrderDto>>> Get() 
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders.AsQueryable());
        }

        /// <summary>
        /// Lấy chi tiết một đơn hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var dto = await _orderService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật một đơn hàng (ví dụ thay đổi địa chỉ, trạng thái)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDto dto)
        {
            if (dto == null || dto.OrderId != id)
                return BadRequest("Order ID mismatch.");

            var existing = await _orderService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _orderService.UpdateAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// Xóa một đơn hàng
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _orderService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _orderService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto create)
        {
            if (create == null || create.Details.Count == 0)
                return BadRequest();

            var created = await _orderService.CreateAsync(create);
            return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }

        [HttpGet("history/{customerId:int}")]
        public async Task<ActionResult<IEnumerable<OrderHistoryDto>>> GetCustomerHistory(int customerId)
        {
            var history = await _orderService.GetHistoryByCustomerAsync(customerId);
            return Ok(history);
        }

    }
}
