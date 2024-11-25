using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.Windows;

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
        private string? statusMessage;  // For showing algorithm results/errors
        
        

        public GraphViewModel(MainViewModel mainViewModel)
        {
            Cities = mainViewModel.Cities;
            Routes = mainViewModel.Routes;

            // Add some test routes if Routes is empty
            if (Routes.Count == 0 && Cities.Count >= 2)
            {
                ShowDebug("Adding test routes");
                // Add a few test routes between existing cities
                for (int i = 0; i < Cities.Count - 1; i++)
                {
                    Routes.Add(new Route
                    {
                        Start = Cities[i],
                        Destination = Cities[i + 1],
                        Distance = 100 // Example distance
                    });
                    ShowDebug($"Added route from {Cities[i].Name} to {Cities[i + 1].Name}");
                }
            }
        }
        
        private void ShowDebug(string message)
        {
            MessageBox.Show(message, "Debug Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearHighlights()
        {
            foreach (var route in Routes)
            {
                route.IsHighlighted = false;
            }
        }

        private void HighlightPath(IEnumerable<Route> path)
        {
            try
            {
                ClearHighlights();
                var count = 0;
                foreach (var route in path)
                {
                    route.IsHighlighted = true;
                    count++;
                    ShowDebug($"Highlighting route from {route.Start.Name} to {route.Destination.Name}");
                }
                ShowDebug($"Highlighted {count} routes");
            }
            catch (Exception ex)
            {
                ShowDebug($"Error in HighlightPath: {ex.Message}");
            }
        }
        
        partial void OnSelectedStartCityChanged(CityNode? value) => FindShortestRouteCommand.NotifyCanExecuteChanged();
        partial void OnSelectedEndCityChanged(CityNode? value) => FindShortestRouteCommand.NotifyCanExecuteChanged();

        private bool CanFindShortestRoute()
        {
            return SelectedStartCity != null && SelectedEndCity != null &&
                   SelectedStartCity != SelectedEndCity;
        }

        private Dictionary<CityNode, List<(CityNode city, Route route)>> BuildAdjacencyList()
        {
            var adjacencyList = new Dictionary<CityNode, List<(CityNode city, Route route)>>();
            
            foreach (var city in Cities)
            {
                adjacencyList[city] = new List<(CityNode city, Route route)>();
            }
            ShowDebug($"Starting to build adjacency list with {Routes.Count} routes");

            foreach (var route in Routes)
            {
                ShowDebug($"Adding route: {route.Start.Name} -> {route.Destination.Name}");
                adjacencyList[route.Start].Add((route.Destination, route));
                adjacencyList[route.Destination].Add((route.Start, route)); // Remove if directed graph
            }

            return adjacencyList;
        }

        [RelayCommand(CanExecute = nameof(CanFindShortestRoute))]
        private void FindShortestRoute()
        {
            try
            {
                if (SelectedStartCity == null || SelectedEndCity == null)
                {
                    ShowDebug("Start or end city is null");
                    return;
                }
                
                ShowDebug($"Finding route from {SelectedStartCity.Name} to {SelectedEndCity.Name}");

                var adjList = BuildAdjacencyList();
                ShowDebug($"Built adjacency list with {adjList.Count} cities");
                var distances = new Dictionary<CityNode, int>();
                var previous = new Dictionary<CityNode, (CityNode city, Route route)?>();
                var unvisited = new HashSet<CityNode>(Cities);

                foreach (var city in Cities)
                {
                    distances[city] = int.MaxValue;
                }

                distances[SelectedStartCity] = 0;

                while (unvisited.Count > 0)
                {
                    var current = unvisited.MinBy(c => distances[c])!;
                    if (current == SelectedEndCity) break;

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

                // Reconstruct path
                var path = new List<Route>();
                var currentCity = SelectedEndCity;
                while (currentCity != SelectedStartCity)
                {
                    if (!previous.ContainsKey(currentCity) || !previous[currentCity].HasValue)
                        return; // No path exists

                    var (prevCity, route) = previous[currentCity].Value;
                    path.Add(route);
                    currentCity = prevCity;
                }
                
                ShowDebug($"Path reconstruction complete. Found {path.Count} segments:");
                foreach (var route in path)
                {
                    ShowDebug($"Path segment: {route.Start.Name} -> {route.Destination.Name}");
                }

                path.Reverse();
                ShowDebug($"About to highlight {path.Count} path segments");
                ShowDebug($"Found path with {path.Count} segments");
                HighlightPath(path);
                StatusMessage = $"Found path with {path.Count} segments and total distance {path.Sum(r => r.Distance)}";
            }
            catch (Exception ex)
            {
                ShowDebug($"Error: {ex.Message}\n\nStack Trace: {ex.StackTrace}");
                StatusMessage = "Error finding route: " + ex.Message;
            }
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
                                // Found a cycle
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

                // Highlight all cycles
                foreach (var cycle in cycles)
                {
                    foreach (var route in cycle)
                    {
                        route.IsHighlighted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDebug($"Error in FindCycles: {ex.Message}");
            }
        }

        [RelayCommand]
        private void FindShortestCycle()
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
                            // Found a cycle back to start
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
            }
        }
    }
}