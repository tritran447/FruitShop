using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using System.Security.Claims;

namespace FruitShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        public CustomerController(ICustomerService service)
            => _service = service;

        [EnableQuery]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var list = _service.GetAllAsync().GetAwaiter().GetResult();
            return Ok(list.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("({key})")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult Get(int key)
        {
            if (User.IsInRole("User") &&
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) != key)
            {
                return Forbid();
            }

            var dto = _service.GetByIdAsync(key).GetAwaiter().GetResult();
            if (dto == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { dto }.AsQueryable()));
        }

        [HttpPost]
        [AllowAnonymous] 
        public async Task<IActionResult> Create([FromBody] CustomerDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { key = created.CustomerId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDto dto)
        {
            if (id != dto.CustomerId)
                return BadRequest();

            if (User.IsInRole("User") &&
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) != id)
            {
                return Forbid();
            }

            return await _service.UpdateAsync(dto)
                ? NoContent()
                : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id)
                ? NoContent()
                : NotFound();
    }
}
