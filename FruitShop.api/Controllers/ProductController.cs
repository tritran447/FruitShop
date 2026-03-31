using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services.Products;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;

namespace FruitShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service) => _service = service;

        // GET api/Product
        [HttpGet]
        [EnableQuery] 
        public IActionResult GetAll()
        {
            var list = _service.GetPagedAsync(1, int.MaxValue).GetAwaiter().GetResult();
            return Ok(list.AsQueryable()); 
        }

        // GET api/Product/5
        // Admin & User
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var dto = _service.GetByIdAsync(id).GetAwaiter().GetResult();
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST api/Product
        // Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] ProductDto dto)
        {
            var created = _service.CreateAsync(dto).GetAwaiter().GetResult();
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        // PUT api/Product/5
        // Admin only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] ProductDto dto)
        {
            if (id != dto.ProductId) return BadRequest();
            var success = _service.UpdateAsync(dto).GetAwaiter().GetResult();
            return success ? NoContent() : NotFound();
        }

        // DELETE api/Product/5
        // Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var success = _service.DeleteAsync(id).GetAwaiter().GetResult();
            return success ? NoContent() : NotFound();
        }

        // GET api/Product/popular?count=3
        // Admin & User
        [HttpGet("popular")]
        public IActionResult GetPopular([FromQuery] int count = 3)
        {
            var list = _service.GetPopularRandomAsync(count).GetAwaiter().GetResult();
            return Ok(list);
        }

        // GET api/Product/related?productId=5&count=4
        // Admin & User
        [HttpGet("related")]
        public IActionResult GetRelated([FromQuery] int productId, [FromQuery] int count = 4)
        {
            var list = _service.GetRelatedProductsAsync(productId, count).GetAwaiter().GetResult();
            return Ok(list);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PaginatedList<ProductDto>>> Search(
            [FromQuery] string name,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name query is required.");

            var result = await _service.SearchByNameAsync(name, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("best-sellers")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBestSellers([FromQuery] int count = 5)
        {
            var list = await _service.GetBestSellersAsync(count);
            return Ok(list);
        }

    }
}
