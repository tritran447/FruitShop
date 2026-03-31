using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FruitShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoriesController(ICategoryService service)
            => _service = service;

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var all = _service.GetAllAsync().Result;
            return Ok(all.AsQueryable());
        }

        // GET api/Categories/5
        // Admin & User
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var dto = _service.GetByIdAsync(id).GetAwaiter().GetResult();
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST api/Categories
        // Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CategoryDto dto)
        {
            var created = _service.CreateAsync(dto).GetAwaiter().GetResult();
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
        }

        // PUT api/Categories/5
        // Admin only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] CategoryDto dto)
        {
            if (id != dto.CategoryId) return BadRequest();

            var success = _service.UpdateAsync(dto).GetAwaiter().GetResult();
            return success ? NoContent() : NotFound();
        }

        // DELETE api/Categories/5
        // Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var success = _service.DeleteAsync(id).GetAwaiter().GetResult();
            return success ? NoContent() : NotFound();
        }
    }
}
