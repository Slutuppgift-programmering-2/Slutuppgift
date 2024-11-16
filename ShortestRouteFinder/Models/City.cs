namespace ShortestRouteFinder.Models;

public class City 
{
   public string? Name { get; set; }
   public double X { get; set; }
   public double Y { get; set; }
   public double Penalty { get; set; }
   public List<string?> ConnectedTo { get; set; }
}