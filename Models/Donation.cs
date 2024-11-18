using System;

namespace SustainableFDPbackend.Models
{
  public class Donation
  {
    public int Id { get; set; }
    public string ItemName { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public int DonorId { get; set; }
    public User Donor { get; set; }
    public bool IsClaimed { get; set; }
  }
}
