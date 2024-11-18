using SustainableFDPbackend.DTOs;
using SustainableFDPbackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustainableFDPbackend.Interfaces
{
  public interface IDonationService
  {
    Task<Donation> AddDonation(DonationDto donationDto, int donorId);
    Task<IEnumerable<Donation>> GetAvailableDonations();
    Task<IEnumerable<Donation>> GetDonationsByDonor(int donorId); // New method
    Task<IEnumerable<DonationClaim>> GetDonationsClaimedByUser(int userId); // New method
    Task<DonationClaim> ClaimDonation(int donationId, int userId);
  }
}
