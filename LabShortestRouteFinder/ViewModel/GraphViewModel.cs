using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace LabShortestRouteFinder.ViewModel
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ObservableCollection<CityNode> _cities;
        private readonly ObservableCollection<Route> _routes;
        private CityNode? _selectedStartCity;
        private CityNode? _selectedEndCity;
        private double _currentPathDistance;

        public ObservableCollection<CityNode> Cities => _cities;
        public ObservableCollection<Route> Routes => _routes;
        public ObservableCollection<Route> PathRoutes { get; } = [];
        
        public CityNode? SelectedStartCity
        {
            get => _selectedStartCity;
            set
            {
                if (_selectedStartCity == value) return;
                _selectedStartCity = value;
                OnPropertyChanged(nameof(SelectedStartCity));
                FindShortestPath();
            }
        }

        public CityNode? SelectedEndCity
        {
            get => _selectedEndCity;
            set
            {
                if (_selectedEndCity == value) return;
                _selectedEndCity = value;
                OnPropertyChanged(nameof(SelectedEndCity));
                FindShortestPath();
            }
        }

        public double CurrentPathDistance
        {
            get => _currentPathDistance;
            set
            {
                if (_currentPathDistance == value) return;
                _currentPathDistance = value;
                OnPropertyChanged(nameof(CurrentPathDistance));
            }
        }

        public GraphViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _cities = mainViewModel.Cities;
            _routes = mainViewModel.Routes;
        }

        private void FindShortestPath()
        {
            if (SelectedStartCity == null || SelectedEndCity == null)
                return;

            Debug.WriteLine($"Finding path from {SelectedStartCity.Name} to {SelectedEndCity.Name}");
            ResetPathHighlighting();

            var shortestPath = DijkstraShortestPath(SelectedStartCity, SelectedEndCity);
            if (shortestPath != null)
            {
                HighlightPath(shortestPath);
                var distance = CalculatePathDistance(shortestPath);
                Debug.WriteLine($"Path found with distance: {distance}");
                CurrentPathDistance = distance;
            }
            else
            {
                Debug.WriteLine("No path found");
                CurrentPathDistance = 0;
            }
        }

        private List<CityNode>? DijkstraShortestPath(CityNode start, CityNode end)
        {
            var distances = new Dictionary<CityNode, double>();
            var previous = new Dictionary<CityNode, CityNode>();
            var unvisited = new HashSet<CityNode>(Cities);

            foreach (var city in Cities)
            {
                distances[city] = double.MaxValue;
            }
            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                var current = unvisited.OrderBy(c => distances[c]).First();
                
                if (current == end)
                    break;

                unvisited.Remove(current);

                var connectedRoutes = Routes.Where(r => 
                    (r.Start == current || r.Destination == current));

                foreach (var route in connectedRoutes)
                {
                    var neighbor = route.Start == current ? route.Destination : route.Start;
                    if (!unvisited.Contains(neighbor))
                        continue;

                    var alt = distances[current] + route.Distance;
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }

            if (!previous.ContainsKey(end))
                return null;

            var path = new List<CityNode>();
            var currentNode = end;
            while (currentNode != null)
            {
                path.Add(currentNode);
                previous.TryGetValue(currentNode, out currentNode);
            }
            path.Reverse();
            return path;
        }

        private void HighlightPath(List<CityNode> path)
        {
            PathRoutes.Clear();

            for (int i = 0; i < path.Count; i++)
            {
                path[i].IsPartOfPath = true;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                var route = Routes.FirstOrDefault(r =>
                    (r.Start == path[i] && r.Destination == path[i + 1]) ||
                    (r.Start == path[i + 1] && r.Destination == path[i]));

                if (route != null)
                {
                    route.IsPartOfPath = true;
                    var pathRoute = new Route
                    {
                        Start = path[i],
                        Destination = path[i + 1],
                        Distance = route.Distance
                    };
                    PathRoutes.Add(pathRoute);
                }
            }
        }

        private void ResetPathHighlighting()
        {
            PathRoutes.Clear();
            
            foreach (var route in Routes)
            {
                route.IsPartOfPath = false;
                route.IsPartOfCycle = false;
            }

            foreach (var city in Cities)
            {
                city.IsPartOfPath = false;
                city.IsPartOfCycle = false;
            }

            CurrentPathDistance = 0;
        }

        private double CalculatePathDistance(List<CityNode> path)
        {
            double totalDistance = 0;
            
            for (int i = 0; i < path.Count - 1; i++)
            {
                var currentCity = path[i];
                var nextCity = path[i + 1];
                
                var route = Routes.FirstOrDefault(r =>
                    (r.Start == currentCity && r.Destination == nextCity) ||
                    (r.Start == nextCity && r.Destination == currentCity));
                
                if (route != null)
                {
                    totalDistance += route.Distance;
                    Debug.WriteLine($"Distance from {currentCity.Name} to {nextCity.Name}: {route.Distance}");
                }
                else
                {
                    Debug.WriteLine($"No route found between {currentCity.Name} and {nextCity.Name}!");
                }
            }
            
            return totalDistance;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
