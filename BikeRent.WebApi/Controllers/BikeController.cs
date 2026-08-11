using BikeRent.Domain.Features.Bike;
using BikeRent.Domain.Models.Bike;
using Microsoft.AspNetCore.Mvc;

namespace BikeRent.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BikeController : ControllerBase
    {
        private readonly BikeService _bikeService;

        public BikeController(BikeService bikeService)
        {
            _bikeService = bikeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBikes()
        {
            var bikes = await _bikeService.GetBikesAsync();
            return Ok(bikes);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableBikes()
        {
            var bikes = await _bikeService.GetAvailableBikesAsync();
            return Ok(bikes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBike(int id)
        {
            var bike = await _bikeService.GetBikeAsync(id);
            if (bike == null) return NotFound("Bike not found.");
            return Ok(bike);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBike([FromBody] BikeCreateRequestModel request)
        {
            var bike = await _bikeService.CreateBikeAsync(request);
            return Ok(bike);
        }

        [HttpPut]
        public async Task<IActionResult> EditBike([FromBody] BikeEditRequestModel request)
        {
            var success = await _bikeService.EditBikeAsync(request);
            if (!success) return NotFound("Bike not found.");
            return Ok("Bike updated successfully.");
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] BikePatchRequestModel request)
        {
            var success = await _bikeService.PatchStatusAsync(request);
            if (!success) return NotFound("Bike not found.");
            return Ok("Bike status updated.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBike(int id)
        {
            var success = await _bikeService.DeleteBikeAsync(id);
            if (!success) return NotFound("Bike not found.");
            return Ok("Bike deleted successfully.");
        }
    }
}
