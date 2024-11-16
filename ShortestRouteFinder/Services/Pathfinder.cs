using System;
using System.Collections.Generic;
using System.Linq;
using ShortestRouteFinder.Models;

namespace ShortestRouteFinder.Services
{
    public class PathFinder
    {
        private readonly List<City> cities;
        private readonly double canvasWidth;
        private readonly double canvasHeight;

        public PathFinder(List<City> cities, double canvasWidth, double canvasHeight)
        {
            this.cities = cities ?? throw new ArgumentNullException(nameof(cities));
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
        }

        public (List<string> Path, double Distance) FindShortestPath(string start, string end)
        {
            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                return (new List<string>(), double.PositiveInfinity);

            var distances = new Dictionary<string, double>();
            var previous = new Dictionary<string, string>();
            var unvisited = new HashSet<string>();

            foreach (var city in cities)
            {
                if (!string.IsNullOrEmpty(city.Name))
                {
                    distances[city.Name] = double.PositiveInfinity;
                    unvisited.Add(city.Name);
                }
            }

            if (!distances.ContainsKey(start))
                return (new List<string>(), double.PositiveInfinity);

            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                var currentNode = unvisited.MinBy(v => distances[v]);
                if (currentNode == null || currentNode == end)
                    break;

                unvisited.Remove(currentNode);
                var currentCity = cities.FirstOrDefault(c => c.Name == currentNode);
                
                if (currentCity?.ConnectedTo == null) continue;

                foreach (var neighbor in currentCity.ConnectedTo)
                {
                    if (string.IsNullOrEmpty(neighbor) || !unvisited.Contains(neighbor))
                        continue;

                    var neighborCity = cities.FirstOrDefault(c => c.Name == neighbor);
                    if (neighborCity == null) continue;

                    double distance = CalculateDistance(currentCity, neighborCity);
                    double totalDistance = distances[currentNode] + distance + neighborCity.Penalty;

                    if (totalDistance < distances[neighbor])
                    {
                        distances[neighbor] = totalDistance;
                        previous[neighbor] = currentNode;
                    }
                }
            }

            if (!previous.ContainsKey(end))
                return (new List<string>(), double.PositiveInfinity);

            var path = new List<string>();
            string? currentPath = end;
            while (currentPath != null)
            {
                path.Add(currentPath);
                previous.TryGetValue(currentPath, out currentPath);
            }
            path.Reverse();

            return (path, distances[end]);
        }

        private double CalculateDistance(City city1, City city2)
        {
            double dx = (city1.X - city2.X) * canvasWidth;
            double dy = (city1.Y - city2.Y) * canvasHeight;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}