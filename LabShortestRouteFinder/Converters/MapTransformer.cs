using LabShortestRouteFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabShortestRouteFinder.Converters
{
    public class MapTransformer
    {
        private readonly double _minLatitude;
        private readonly double _maxLatitude;
        private readonly double _minLongitude;
        private readonly double _maxLongitude;
        private readonly int _windowWidth;
        private readonly int _windowHeight;

        public MapTransformer(double minLatitude, double maxLatitude, double minLongitude, double maxLongitude, int windowWidth, int windowHeight)
        {
            _minLatitude = minLatitude;
            _maxLatitude = maxLatitude;
            _minLongitude = minLongitude;
            _maxLongitude = maxLongitude;
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }

        public MapTransformer(MapWin mapWin)
        {
            _minLatitude = mapWin.MinLat;
            _maxLatitude = mapWin.MaxLat;
            _minLongitude = mapWin.MinLon;
            _maxLongitude = mapWin.MaxLon;
            _windowWidth = mapWin.WindowWidth;
            _windowHeight = mapWin.WindowHeight;
        }

        public (int x, int y) TransformToScreenPosition(double latitude, double longitude)
        {
            var x = (int)((longitude - _minLongitude) / (_maxLongitude - _minLongitude) * _windowWidth);
            var y = (int)((_maxLatitude - latitude) / (_maxLatitude - _minLatitude) * _windowHeight);
            return (x, y);
        }

        public List<CityNode> TransformCities(List<CityNode> cities)
        {
            foreach (var city in cities)
            {
                var (x, y) = TransformToScreenPosition(city.Latitude, city.Longitude);
                city.X = x;
                city.Y = y;
            }
            return cities;
        }

        public ObservableCollection<CityNode> TransformCities(ObservableCollection<CityNode> cities)
        {
            foreach (var city in cities)
            {
                var (x, y) = TransformToScreenPosition(city.Latitude, city.Longitude);
                city.X = x;
                city.Y = y;
            }
            return cities;
        }

    }
}
