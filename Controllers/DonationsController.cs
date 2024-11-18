using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SustainableFDPbackend.DTOs;
using SustainableFDPbackend.Interfaces;
using SustainableFDPbackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SustainableFDPbackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DonationsController : ControllerBase
  {
    private readonly IDonationService _donationService;
    private readonly ILogger<DonationsController> _logger;

    public DonationsController(IDonationService donationService, ILogger<DonationsController> logger)
    {
      _donationService = donationService;
      _logger = logger;  // Inject logger into the controller
    }

    [HttpPost]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> AddDonation(DonationDto donationDto)
    {
      // Log all claims
      foreach (var claim in User.Claims)
      {
        _logger.LogInformation("Claim Type: {ClaimType}, Claim Value: {ClaimValue}", claim.Type, claim.Value);
      }

      // Retrieve the user ID claim using the correct claim type
      var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

      // Log the userIdClaim
      _logger.LogInformation("User ID claim extracted for adding donation: {UserIdClaim}", userIdClaim);

      // Check if the claim is valid and can be parsed as an integer
      if (userIdClaim == null || !int.TryParse(userIdClaim, out int donorId))
      {
        _logger.LogWarning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
        return BadRequest(new { message = "Invalid user ID" });
      }

      // Proceed with adding the donation
      var newDonation = await _donationService.AddDonation(donationDto, donorId);
      return Ok(newDonation);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Donation>>> GetAvailableDonations()
    {
      var donations = await _donationService.GetAvailableDonations();
      return Ok(donations);
    }

    [HttpGet("my-donations")]
    [Authorize(Roles = "Donor")]
    public async Task<ActionResult<IEnumerable<Donation>>> GetDonationsByDonor()
    {
      // Retrieve the user ID claim using the correct claim type
      var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

      // Log the userIdClaim
      _logger.LogInformation("User ID claim extracted for retrieving donations: {UserIdClaim}", userIdClaim);

      // Check if the claim is valid and can be parsed as an integer
      if (userIdClaim == null || !int.TryParse(userIdClaim, out int donorId))
      {
        _logger.LogWarning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
        return BadRequest(new { message = "Invalid user ID" });
      }

      var donations = await _donationService.GetDonationsByDonor(donorId);
      return Ok(donations);
    }

    [HttpGet("my-claimed-donations")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<IEnumerable<DonationClaim>>> GetDonationsClaimedByUser()
    {
      // Retrieve the user ID claim using the correct claim type
      var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

      // Log the userIdClaim
      _logger.LogInformation("User ID claim extracted for retrieving claimed donations: {UserIdClaim}", userIdClaim);

      // Check if the claim is valid and can be parsed as an integer
      if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
      {
        _logger.LogWarning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
        return BadRequest(new { message = "Invalid user ID" });
      }

      var donationClaims = await _donationService.GetDonationsClaimedByUser(userId);
      return Ok(donationClaims);
    }

    [HttpPost("{donationId}/claim")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ClaimDonation(int donationId)
    {
      // Log all claims
      foreach (var claim in User.Claims)
      {
        _logger.LogInformation("Claim Type: {ClaimType}, Claim Value: {ClaimValue}", claim.Type, claim.Value);
      }

      // Retrieve the user ID claim using the correct claim type
      var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

      // Log the userIdClaim
      _logger.LogInformation("User ID claim extracted for claiming donation: {UserIdClaim}", userIdClaim);

      // Check if the claim is valid and can be parsed as an integer
      if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
      {
        _logger.LogWarning("Invalid user ID claim: {UserIdClaim} for claiming donation", userIdClaim);
        return BadRequest(new { message = "Invalid user ID" });
      }

      var donationClaim = await _donationService.ClaimDonation(donationId, userId);
      if (donationClaim == null)
      {
        return BadRequest(new { message = "Donation not available or already claimed" });
      }
      return Ok(donationClaim);
    }
  }
}
