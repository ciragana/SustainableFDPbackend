using System;

namespace SustainableFDPbackend.Models;

public class Restaurant
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Location { get; set; }
  public int OwnerId { get; set; }
  public User Owner { get; set; }
}
