using System;

namespace SustainableFDPbackend.Models
{
  public class DonationClaim
  {
    public int Id { get; set; }
    public int DonationId { get; set; }
    public Donation Donation { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime ClaimedAt { get; set; }
  }
}
