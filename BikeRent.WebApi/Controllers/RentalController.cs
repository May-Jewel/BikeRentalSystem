using BikeRent.Domain.Features.Rental;
using BikeRent.Domain.Models.Rental;
using Microsoft.AspNetCore.Mvc;

namespace BikeRent.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController : ControllerBase
    {
        private readonly RentalService _rentalService;

        public RentalController(RentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost("rent")]
        public async Task<IActionResult> RentBike([FromBody] RentalCreateRequestModel request)
        {
            var rental = await _rentalService.RentBikeAsync(request);
            if (rental == null) return BadRequest("Bike is unavailable or does not exist.");
            return Ok(rental);
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBike([FromBody] RentalReturnRequestModel request)
        {
            var rental = await _rentalService.ReturnBikeAsync(request);
            if (rental == null) return BadRequest("Rental record not found or already completed.");
            return Ok(rental);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetRentalHistory()
        {
            var history = await _rentalService.GetRentalHistoryAsync();
            return Ok(history);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRental(int id)
        {
            var rental = await _rentalService.GetRentalAsync(id);
            if (rental == null) return NotFound("Rental not found.");
            return Ok(rental);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRental(int id)
        {
            var success = await _rentalService.DeleteRentalAsync(id);
            if (!success) return NotFound("Rental not found.");
            return Ok("Rental deleted successfully.");
        }
    }
}
