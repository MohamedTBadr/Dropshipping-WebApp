using BAL.DTOs.CategoryDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await categoryService.GetAllCategories());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await categoryService.GetCategoryById(id));

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDTO createDTO)
        {
            await categoryService.AddCategory(createDTO);
            return Created();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDTO updateDTO)
        {
            await categoryService.UpdateCategory(updateDTO);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await categoryService.DeleteCategory(id);
            return NoContent();
        }
    }
}
