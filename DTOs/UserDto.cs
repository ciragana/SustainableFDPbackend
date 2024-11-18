using SustainableFDPbackend.Models;

namespace SustainableFDPbackend.DTOs
{
  public class UserDto
  {
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; } // Use Role enum
  }
}
