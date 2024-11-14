using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LabShortestRouteFinder.Model
{
    public class Route
    {
        public required CityNode Start {  get; set; }
        public required CityNode Destination { get; set; }
        public int Distance { get; set; }
    }
}
