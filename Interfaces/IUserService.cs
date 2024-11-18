using SustainableFDPbackend.DTOs;
using SustainableFDPbackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustainableFDPbackend.Interfaces
{
  public interface IUserService
  {
    Task<User> Register(UserDto userDto, Role role);
    Task<LoginResponseDto> Login(LoginDto loginDto);

    Task<bool> AssignRole(int userId, Role role);
    Task<IEnumerable<User>> GetUsersByRole(string role);
    Task<int> GetDonationCountByDonor(int donorId);
    Task<int> GetDonationClaimCountByUser(int userId);
    Task<Dictionary<string, IEnumerable<User>>> GetAllUsersGroupedByRole();
  }
}
