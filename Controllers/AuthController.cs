using Microsoft.AspNetCore.Mvc;
using SustainableFDPbackend.Data;
using SustainableFDPbackend.DTOs;
using SustainableFDPbackend.Interfaces;
using SustainableFDPbackend.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SustainableFDPbackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public AuthController(IUserService userService, AppDbContext context)
    {
      _userService = userService;
      _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
      // Assign Admin role to the first user
      var role = userDto.Role;
      if (!_context.Users.Any())
      {
        role = Role.Admin;
      }
      else
      {
        // Validate the role for subsequent users
        if (role != Role.User && role != Role.Donor)
        {
          return BadRequest(new { message = "Invalid role. Allowed roles are 'User' and 'Donor'." });
        }
      }

      var user = await _userService.Register(userDto, role);
      return Ok(new
      {
        message = "User registered successfully",
        user = new
        {
          user.Id,
          user.Username,
          user.Email,
          user.Password,
          Role = user.Role.ToString()
        }
      });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      var loginResponse = await _userService.Login(loginDto);
      if (loginResponse == null)
      {
        return Unauthorized(new { message = "Invalid credentials" });
      }

      return Ok(loginResponse);
    }
  }
}
