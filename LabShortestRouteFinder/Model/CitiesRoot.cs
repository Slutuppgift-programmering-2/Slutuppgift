using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabShortestRouteFinder.Model
{
    public class CitiesRoot
    {
        public RectangleCoordinates Rectangle { get; set; }

        public List<CityNode> Cities { get; set; }
    }
}
