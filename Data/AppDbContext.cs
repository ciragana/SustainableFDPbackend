using Microsoft.EntityFrameworkCore;
using SustainableFDPbackend.Models;

namespace SustainableFDPbackend.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<DonationClaim> DonationClaims { get; set; }
  }
}
