using BAL.DTOs.BrandDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BrandsController(IBrandService brandService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId)
        {
            if (categoryId.HasValue)
            {
                var brands = await brandService.GetBrandsByCategoryId(categoryId.Value);
                return Ok(brands);
            }
            else
            {
                var brands = await brandService.GetAllAsync();
                return Ok(brands);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id) => Ok(await brandService.GetByIdAsync(id));


        [HttpPost]
        public async Task<IActionResult> Add([FromForm] BrandCreateDTO createDTO)
        {
            await brandService.AddAsync(createDTO);
            return Created();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await brandService.DeleteAsync(id);
            return NoContent();
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromForm] BrandUpdateDTO updateDTO)
        {
            await brandService.UpdateAsync(updateDTO);
            return Ok();
        }
    }
}
