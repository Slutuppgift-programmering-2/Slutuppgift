using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LabShortestRouteFinder.ViewModel
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ObservableCollection<CityNode> _cities;
        private CityNode? _selectedStartCity;
        private CityNode? _selectedEndCity;
        private double _currentPathDistance;

        public ObservableCollection<CityNode> Cities => _cities;
        
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
        }

        private void FindShortestPath()
        {
            if (SelectedStartCity == null || SelectedEndCity == null)
                return;

            // Reset previous path
            ResetPathHighlighting();

            var shortestPath = DijkstraShortestPath(SelectedStartCity, SelectedEndCity);
            if (shortestPath != null)
            {
                HighlightPath(shortestPath);
                CurrentPathDistance = CalculatePathDistance(shortestPath);
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

                // Get all neighboring cities from MainViewModel's routes
                var connectedRoutes = _mainViewModel.Routes.Where(r => r.Start == current || r.Destination == current);
                
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

            // Reconstruct the path
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
            for (int i = 0; i < path.Count; i++)
            {
                path[i].IsPartOfPath = true;
            }
        }

        private void ResetPathHighlighting()
        {
            foreach (var city in Cities)
            {
                city.IsPartOfPath = false;
            }
            CurrentPathDistance = 0;
        }

        private double CalculatePathDistance(List<CityNode> path)
        {
            double distance = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var route = _mainViewModel.Routes.FirstOrDefault(r =>
                    (r.Start == path[i] && r.Destination == path[i + 1]) ||
                    (r.Start == path[i + 1] && r.Destination == path[i]));

                if (route != null)
                    distance += route.Distance;
            }
            return distance;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}