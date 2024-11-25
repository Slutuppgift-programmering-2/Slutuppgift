using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;

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
            ClearHighlights();
            foreach (var route in path)
            {
                route.IsHighlighted = true;
            }
        }

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

            foreach (var route in Routes)
            {
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
                if (SelectedStartCity == null || SelectedEndCity == null) return;

                var adjList = BuildAdjacencyList();
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

                path.Reverse();
                HighlightPath(path);
                StatusMessage = $"Found path with {path.Count} segments and total distance {path.Sum(r => r.Distance)}";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error finding route: " + ex.Message;
            }
        }

        [RelayCommand]
        private void FindCycles()
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