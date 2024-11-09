using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabShortestRouteFinder.Model
{
    public class Route
    {
        public required CityNode Start { get; set; }
        public required CityNode Destination { get; set; }
        public int Distance { get; set; }
    }
}
