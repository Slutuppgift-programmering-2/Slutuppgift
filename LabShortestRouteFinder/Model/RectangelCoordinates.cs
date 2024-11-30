using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabShortestRouteFinder.Model
{
    public class RectangleCoordinates
    {
        public Coordinate NorthWest { get; set; }
        public Coordinate NorthEast { get; set; }
        public Coordinate SouthWest { get; set; }
        public Coordinate SouthEast { get; set; }
    }

    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
