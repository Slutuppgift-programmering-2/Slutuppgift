using System.Collections.Generic;

namespace ShortestRouteFinder.Models;

public class City
{
    public required string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Penalty { get; set; }
    public required List<string> ConnectedTo { get; set; } = [];
}