using ShortestRouteFinder.Models;

namespace ShortestRouteFinder.Services
{
    public class PathFinder
    {
        private readonly List<City>? _cities;
        private readonly double _canvasWidth;
        private readonly double _canvasHeight;

        public PathFinder(List<City>? cities, double canvasWidth, double canvasHeight)
        {
            this._cities = cities;
            this._canvasWidth = canvasWidth;
            this._canvasHeight = canvasHeight;
        }

        public (List<string?>? path, double) FindShortestPath(string? start, string? end)
        {
            var distances = new Dictionary<string?, double>();
            var previous = new Dictionary<string?, string?>();
            var unvisited = new HashSet<string?>();

            foreach (var city in _cities)
            {
                distances[city.Name] = double.PositiveInfinity;
                unvisited.Add(city.Name);
            }
            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                string? current = unvisited.MinBy(v => distances[v]);
                
                if (current == end)
                    break;

                unvisited.Remove(current);
                var currentCity = _cities.First(c => c.Name == current);

                foreach (var neighbor in currentCity.ConnectedTo)
                {
                    if (!unvisited.Contains(neighbor))
                        continue;

                    var neighborCity = _cities.First(c => c.Name == neighbor);
                    double distance = CalculateDistance(currentCity, neighborCity);
                    double totalDistance = distances[current] + distance + neighborCity.Penalty;

                    if (totalDistance < distances[neighbor])
                    {
                        distances[neighbor] = totalDistance;
                        previous[neighbor] = current;
                    }
                }
            }

            if (!previous.ContainsKey(end))
                return (null, double.PositiveInfinity)!;

            var path = new List<string?>();
            string? curr = end;
            while (curr != null)
            {
                path.Add(curr);
                previous.TryGetValue(curr, out curr);
            }
            path.Reverse();

            return (path, distances[end]);
        }

        private double CalculateDistance(City city1, City city2)
        {
            double dx = (city1.X - city2.X) * _canvasWidth;
            double dy = (city1.Y - city2.Y) * _canvasHeight;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}