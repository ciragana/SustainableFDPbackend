using Microsoft.EntityFrameworkCore;
using SustainableFDPbackend.Data;
using SustainableFDPbackend.DTOs;
using SustainableFDPbackend.Interfaces;
using SustainableFDPbackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustainableFDPbackend.Services
{
  public class DonationService : IDonationService
  {
    private readonly AppDbContext _context;

    public DonationService(AppDbContext context)
    {
      _context = context;
    }

    public async Task<Donation> AddDonation(DonationDto donationDto, int donorId)
    {
      var donation = new Donation
      {
        ItemName = donationDto.ItemName,
        Description = donationDto.Description,
        Quantity = donationDto.Quantity,
        DonorId = donorId,
        IsClaimed = false
      };

      _context.Donations.Add(donation);
      await _context.SaveChangesAsync();
      return donation;
    }

    public async Task<IEnumerable<Donation>> GetAvailableDonations()
    {
      return await _context.Donations
          .Include(d => d.Donor)
          .Where(d => !d.IsClaimed)
          .ToListAsync();
    }

    public async Task<IEnumerable<Donation>> GetDonationsByDonor(int donorId)
    {
      return await _context.Donations
          .Include(d => d.Donor)
          .Where(d => d.DonorId == donorId)
          .ToListAsync();
    }

    public async Task<IEnumerable<DonationClaim>> GetDonationsClaimedByUser(int userId)
    {
      return await _context.DonationClaims
          .Include(dc => dc.Donation)
              .ThenInclude(d => d.Donor)
          .Include(dc => dc.User)
          .Where(dc => dc.UserId == userId)
          .ToListAsync();
    }

    public async Task<DonationClaim> ClaimDonation(int donationId, int userId)
    {
      var donation = await _context.Donations.FindAsync(donationId);
      if (donation == null || donation.IsClaimed)
      {
        return null;
      }

      donation.IsClaimed = true;
      _context.Donations.Update(donation);

      var donationClaim = new DonationClaim
      {
        DonationId = donationId,
        UserId = userId,
        ClaimedAt = DateTime.UtcNow
      };

      _context.DonationClaims.Add(donationClaim);
      await _context.SaveChangesAsync();

      // Eagerly load the related entities
      return await _context.DonationClaims
          .Include(dc => dc.Donation)
              .ThenInclude(d => d.Donor)
          .Include(dc => dc.User)
          .FirstOrDefaultAsync(dc => dc.Id == donationClaim.Id);
    }
  }
}
