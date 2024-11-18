using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SustainableFDPbackend.Interfaces;
using SustainableFDPbackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustainableFDPbackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(Roles = "Admin")]
  public class AdminController : ControllerBase
  {
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpGet("users-by-role/{role}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string role)
    {
      var users = await _userService.GetUsersByRole(role);
      return Ok(users);
    }

    [HttpGet("donation-count-by-donor/{donorId}")]
    public async Task<ActionResult<int>> GetDonationCountByDonor(int donorId)
    {
      var count = await _userService.GetDonationCountByDonor(donorId);
      return Ok(count);
    }

    [HttpGet("donation-claim-count-by-user/{userId}")]
    public async Task<ActionResult<int>> GetDonationClaimCountByUser(int userId)
    {
      var count = await _userService.GetDonationClaimCountByUser(userId);
      return Ok(count);
    }

    [HttpGet("all-users")]
    public async Task<ActionResult<Dictionary<string, IEnumerable<User>>>> GetAllUsersGroupedByRole()
    {
      var usersGroupedByRole = await _userService.GetAllUsersGroupedByRole();
      return Ok(usersGroupedByRole);
    }
  }
}
