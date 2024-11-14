using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabShortestRouteFinder.Model
{
    public class MapWin
    {
        public MapWin(double minLat, double maxLat, double minLon, double maxLon, int windowWidth, int windowHeight)
        {
            MinLat = minLat;
            MaxLat = maxLat;
            MinLon = minLon;
            MaxLon = maxLon;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public double MinLat { get; }
        public double MaxLat { get; }
        public double MinLon { get; }
        public double MaxLon { get; }
        public int WindowWidth { get; }
        public int WindowHeight { get; }
    }
}
