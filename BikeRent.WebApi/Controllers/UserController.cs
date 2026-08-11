using BikeRent.Domain.Features.User;
using BikeRent.Domain.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace BikeRent.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] UserEditRequestModel request)
        {
            var success = await _userService.EditUserAsync(request);
            if (!success) return NotFound("User not found or phone number is already in use.");
            return Ok("User updated successfully.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound("User not found.");
            return Ok("User deleted successfully.");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestModel request)
        {
            var result = await _userService.RegisterAsync(request);
            if (result == null) return BadRequest("User already exists or registration failed.");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestModel request)
        {
            var user = await _userService.LoginAsync(request);
            if (user == null) return Unauthorized("Invalid phone number or password.");
            return Ok(user);
        }
    }
}
