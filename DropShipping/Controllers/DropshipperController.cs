using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BAL.DTOs.DropshipperDTOs;

namespace PAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropshipperController : ControllerBase
    {
        private readonly IDropshipperService _dropshipperService;

        public DropshipperController(IDropshipperService dropshipperService)
        {
            _dropshipperService = dropshipperService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDropshippers()
        {
            var dropshippers = await _dropshipperService.GetAllDropshippersAsync();
            return Ok(dropshippers);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetDropshipperById(string userId)
        {
            var dropshipper = await _dropshipperService.GetDropshipperByIdAsync(userId);
            if (dropshipper == null)
                return NotFound();

            return Ok(dropshipper);
        }

        [HttpGet("Wallet/{userId}")]
        public async Task<IActionResult> GetDropshipperWalletById(string userId)
        {
            var dropshipper = await _dropshipperService.GetDropshipperWalletByIdAsync(userId);
            if (dropshipper == null)
                return NotFound();

            return Ok(dropshipper);
        }


        [HttpPost]
        public  Task<IActionResult> CreateDropshipper([FromBody] DropshipperDto dropshipperDto)
        {
            var createdDropshipper =  _dropshipperService.CreateDropshipperAsync(dropshipperDto);
            return createdDropshipper.ContinueWith<IActionResult>(t => 
            {
                if (t.IsFaulted)
                {
                    return BadRequest(t.Exception?.Message);
                }
                return Ok();
            });
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateDropshipper(string userId, [FromBody] DropshipperUpdate dropshipperDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dropshipperDto.Id = userId;

            var updatedDropshipper = await _dropshipperService.UpdateDropshipperAsync(dropshipperDto);

            if (updatedDropshipper == null)
                return NotFound();

            return Ok(updatedDropshipper);
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteDropshipper(string userId)
        {
            var existingDropshipper = await _dropshipperService.GetDropshipperByIdAsync(userId);
            if (existingDropshipper == null)
                return NotFound();
            await _dropshipperService.DeleteDropshipperAsync(userId);
            return NoContent();
        }
    }
}
