using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace LabShortestRouteFinder.ViewModel
{
    public partial class GraphViewModel : ObservableObject
    {
        public ObservableCollection<CityNode> Cities { get; }
        public ObservableCollection<Route> Routes { get; }

        [ObservableProperty]
        private CityNode? selectedStartCity;

        [ObservableProperty]
        private CityNode? selectedEndCity;
        
        [ObservableProperty]
        private string? statusMessage;
        
        public GraphViewModel(MainViewModel mainViewModel)
        {
            Cities = mainViewModel.Cities;
            Routes = mainViewModel.Routes;

            if (Routes.Count == 0 && Cities.Count >= 2)
            {
                LoadRoutesFromFile();
            }
        }
        
        private void LoadRoutesFromFile()
        {
            try
            {
                string jsonContent = File.ReadAllText(Path.Combine("Resources", "routes.json"));
                var routeData = JsonSerializer.Deserialize<RouteData>(jsonContent) 
                    ?? new RouteData { routes = new List<RouteInfo>() };

                foreach (var routeInfo in routeData.routes)
                {
                    var startCity = Cities.FirstOrDefault(c => c.Name == routeInfo.from);
                    var endCity = Cities.FirstOrDefault(c => c.Name == routeInfo.to);

                    if (startCity != null && endCity != null)
                    {
                        Routes.Add(new Route
                        {
                            Start = startCity,
                            Destination = endCity,
                            Distance = routeInfo.distance
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load routes: {ex.Message}";
            }
        }

        private void ClearHighlights() => 
            Routes.ToList().ForEach(r => r.IsHighlighted = false);

        private void HighlightPath(IEnumerable<Route> path)
        {
            if (path == null) return;

            // First clear any existing highlights
            ClearHighlights();

            // Build a string to show the complete path
            var pathDescription = new System.Text.StringBuilder();
            var totalDistance = 0;
            var isFirst = true;

            foreach (var route in path)
            {
                route.IsHighlighted = true;
        
                // Build the path description
                if (isFirst)
                {
                    pathDescription.Append(route.Start.Name);
                    isFirst = false;
                }
                pathDescription.Append($" → {route.Destination.Name}");
                totalDistance += route.Distance;
            }

            // Update status message with the complete path and total distance
            StatusMessage = $"Path: {pathDescription}\nTotal Distance: {totalDistance} km";
        }
        
        partial void OnSelectedStartCityChanged(CityNode? value) => 
            FindShortestRouteCommand.NotifyCanExecuteChanged();
            
        partial void OnSelectedEndCityChanged(CityNode? value) => 
            FindShortestRouteCommand.NotifyCanExecuteChanged();

        private bool CanFindShortestRoute() =>
            SelectedStartCity != null && 
            SelectedEndCity != null &&
            SelectedStartCity != SelectedEndCity;

        private Dictionary<CityNode, List<(CityNode city, Route route)>> BuildAdjacencyList()
        {
            var adjacencyList = Cities.ToDictionary(
                city => city,
                _ => new List<(CityNode city, Route route)>()
            );

            foreach (var route in Routes)
            {
                adjacencyList[route.Start].Add((route.Destination, route));
                adjacencyList[route.Destination].Add((route.Start, route));
            }

            return adjacencyList;
        }

        [RelayCommand(CanExecute = nameof(CanFindShortestRoute))]
        private void FindShortestRoute()
        {
            try
            {
                if (SelectedStartCity == null || SelectedEndCity == null) return;

                var path = FindShortestPath(SelectedStartCity, SelectedEndCity);
                
                if (path.Any())
                {
                    HighlightPath(path);
                    //StatusMessage = $"Found path with {path.Count} segments and total distance {path.Sum(r => r.Distance)}";
                }
                else
                {
                    StatusMessage = "No path found between selected cities";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error finding route: {ex.Message}";
            }
        }

        private List<Route> FindShortestPath(CityNode start, CityNode end)
        {
            var adjList = BuildAdjacencyList();
            var distances = new Dictionary<CityNode, int>();
            var previous = new Dictionary<CityNode, (CityNode city, Route route)?>();
            var unvisited = new HashSet<CityNode>(Cities);

            foreach (var city in Cities)
            {
                distances[city] = int.MaxValue;
            }
            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                var current = unvisited.MinBy(c => distances[c])!;
                if (current == end) break;

                unvisited.Remove(current);

                foreach (var (neighbor, route) in adjList[current])
                {
                    if (!unvisited.Contains(neighbor)) continue;

                    var alt = distances[current] + route.Distance;
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = (current, route);
                    }
                }
            }

            return ReconstructPath(start, end, previous);
        }

        private static List<Route> ReconstructPath(
            CityNode start, 
            CityNode end, 
            Dictionary<CityNode, (CityNode city, Route route)?> previous)
        {
            var path = new List<Route>();
            var currentCity = end;

            while (currentCity != start)
            {
                if (!previous.ContainsKey(currentCity) || !previous[currentCity].HasValue)
                    return new List<Route>();

                var (prevCity, route) = previous[currentCity].Value;
                path.Add(route);
                currentCity = prevCity;
            }

            path.Reverse();
            return path;
        }

        [RelayCommand]
        private void FindCycles()
        {
            try
            {
                ClearHighlights();
                var adjList = BuildAdjacencyList();
                var visited = new HashSet<CityNode>();
                var stack = new Stack<CityNode>();
                var cycles = new List<List<Route>>();

                void DFS(CityNode current, CityNode? parent)
                {
                    visited.Add(current);
                    stack.Push(current);

                    foreach (var (neighbor, route) in adjList[current])
                    {
                        if (neighbor == parent) continue;

                        if (visited.Contains(neighbor))
                        {
                            if (stack.Contains(neighbor))
                            {
                                var cycle = new List<Route>();
                                var cycleStack = new Stack<CityNode>(stack);
                                var cycleCity = cycleStack.Pop();
                                
                                while (cycleStack.Count > 0 && cycleCity != neighbor)
                                {
                                    var nextCity = cycleStack.Pop();
                                    cycle.Add(Routes.First(r =>
                                        (r.Start == cycleCity && r.Destination == nextCity) ||
                                        (r.Start == nextCity && r.Destination == cycleCity)));
                                    cycleCity = nextCity;
                                }

                                cycle.Add(route);
                                cycles.Add(cycle);
                            }
                        }
                        else
                        {
                            DFS(neighbor, current);
                        }
                    }

                    stack.Pop();
                }

                foreach (var city in Cities)
                {
                    if (!visited.Contains(city))
                    {
                        DFS(city, null);
                    }
                }

                foreach (var cycle in cycles)
                {
                    foreach (var route in cycle)
                    {
                        route.IsHighlighted = true;
                    }
                }
                
                StatusMessage = $"Found {cycles.Count} cycles";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error finding cycles: {ex.Message}";
            }
        }

        [RelayCommand]
        private void FindShortestCycle()
        {
            try
            {
                var shortestCycle = new List<Route>();
                var minDistance = int.MaxValue;

                foreach (var startCity in Cities)
                {
                    var visited = new HashSet<CityNode> { startCity };
                    var adjList = BuildAdjacencyList();
                    
                    void FindCycles(CityNode current, List<Route> currentPath, int currentDistance)
                    {
                        foreach (var (neighbor, route) in adjList[current])
                        {
                            if (neighbor == startCity && currentPath.Count > 2)
                            {
                                var totalDistance = currentDistance + route.Distance;
                                if (totalDistance < minDistance)
                                {
                                    minDistance = totalDistance;
                                    shortestCycle = new List<Route>(currentPath) { route };
                                }
                            }
                            else if (!visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
                                currentPath.Add(route);
                                FindCycles(neighbor, currentPath, currentDistance + route.Distance);
                                currentPath.RemoveAt(currentPath.Count - 1);
                                visited.Remove(neighbor);
                            }
                        }
                    }

                    FindCycles(startCity, new List<Route>(), 0);
                }

                if (shortestCycle.Any())
                {
                    HighlightPath(shortestCycle);
                    StatusMessage = $"Found shortest cycle with distance {minDistance}";
                }
                else
                {
                    StatusMessage = "No cycles found";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error finding shortest cycle: {ex.Message}";
            }
        }
    }

    public class RouteData
    {
        public List<RouteInfo> routes { get; set; } = new();
    }

    public class RouteInfo
    {
        public required string from { get; set; }
        public required string to { get; set; }
        public int distance { get; set; }
    }
}