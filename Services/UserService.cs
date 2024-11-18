using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SustainableFDPbackend.Data;
using SustainableFDPbackend.DTOs;
using SustainableFDPbackend.Interfaces;
using SustainableFDPbackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SustainableFDPbackend.Services
{
  public class UserService : IUserService
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<User> Register(UserDto userDto, Role role)
    {
      var user = new User
      {
        Username = userDto.Username,
        Email = userDto.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
        Role = role
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return user;
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto)
    {
      var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
      if (dbUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, dbUser.Password))
      {
        return null;
      }

      var token = GenerateJwtToken(dbUser);

      return new LoginResponseDto
      {
        Token = token,
        Id = dbUser.Id,
        Username = dbUser.Username,
        Email = dbUser.Email,
        Role = dbUser.Role.ToString()
      };
    }

    public async Task<bool> AssignRole(int userId, Role role)
    {
      var user = await _context.Users.FindAsync(userId);
      if (user == null)
      {
        return false;
      }

      user.Role = role;
      _context.Users.Update(user);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<IEnumerable<User>> GetUsersByRole(string role)
    {
      return await _context.Users.Where(u => u.Role.ToString() == role).ToListAsync();
    }

    public async Task<int> GetDonationCountByDonor(int donorId)
    {
      return await _context.Donations.CountAsync(d => d.DonorId == donorId);
    }

    public async Task<int> GetDonationClaimCountByUser(int userId)
    {
      return await _context.DonationClaims.CountAsync(dc => dc.UserId == userId);
    }

    public async Task<Dictionary<string, IEnumerable<User>>> GetAllUsersGroupedByRole()
    {
      var users = await _context.Users.ToListAsync();
      return users.GroupBy(u => u.Role.ToString())
                  .ToDictionary(g => g.Key, g => g.Select(u => new User
                  {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Password = u.Password,
                    Role = u.Role
                  }));
    }

    private string GenerateJwtToken(User user)
    {
      var claims = new[]
      {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Add userId to claims
            };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddHours(1),
          signingCredentials: creds);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
