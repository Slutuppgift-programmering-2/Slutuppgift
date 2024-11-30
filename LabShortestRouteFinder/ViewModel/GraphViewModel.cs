using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabShortestRouteFinder.Model;

namespace LabShortestRouteFinder.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    [ObservableProperty] private CityNode? _selectedEndCity;

    [ObservableProperty] private CityNode? _selectedStartCity;

    [ObservableProperty] private string? _statusMessage;

    public GraphViewModel(MainViewModel mainViewModel)
    {
        Cities = mainViewModel.Cities;
        Routes = mainViewModel.Routes;

        if (Routes.Count == 0 && Cities.Count >= 2) LoadRoutesFromFile();
    }

    public ObservableCollection<CityNode> Cities { get; }
    public ObservableCollection<Route> Routes { get; }

    private void LoadRoutesFromFile()
    {
        try
        {
            var jsonContent = File.ReadAllText(Path.Combine("Resources", "routes.json"));
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var routes = JsonSerializer.Deserialize<List<RouteInfo>>(jsonContent, options)
                         ?? new List<RouteInfo>();

            foreach (var routeInfo in routes)
            {
                var startCity = Cities.FirstOrDefault(c => c.Name == routeInfo.From);
                var endCity = Cities.FirstOrDefault(c => c.Name == routeInfo.To);

                if (startCity != null && endCity != null)
                    Routes.Add(new Route
                    {
                        Start = startCity,
                        Destination = endCity,
                        Distance = routeInfo.Distance,
                        Cost = routeInfo.Cost,
                        HighlightedColours = new List<string>()
                    });
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load routes: {ex.Message}";
        }
    }

    private void ClearHighlights()
    {
        Routes.ToList().ForEach(r => r.IsHighlighted = false);
    }

    private void HighlightPath(IEnumerable<Route>? path)
    {
        if (path == null) return;

        ClearHighlights();

        var pathDescription = new StringBuilder();
        var totalDistance = 0;
        var totalCost = 0;
        var isFirst = true;

        foreach (var route in path)
        {
            route.IsHighlighted = true;

            if (isFirst)
            {
                pathDescription.Append(route.Start.Name);
                isFirst = false;
            }

            pathDescription.Append($" → {route.Destination.Name}");
            totalDistance += route.Distance;
            totalCost += route.Cost;
        }
    }

    partial void OnSelectedStartCityChanged(CityNode? value)
    {
        FindShortestRouteCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedEndCityChanged(CityNode? value)
    {
        FindShortestRouteCommand.NotifyCanExecuteChanged();
    }

    private bool CanFindShortestRoute()
    {
        return SelectedStartCity != null &&
               SelectedEndCity != null &&
               SelectedStartCity != SelectedEndCity;
    }

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
                HighlightPath(path);
            //StatusMessage = $"Found path with {path.Count} segments and total distance {path.Sum(r => r.Distance)}";
            else
                StatusMessage = "No path found between selected cities";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error finding route: {ex.Message}";
        }
    }

    private List<Route> FindShortestPath(CityNode start, CityNode end)
    {
        var adjList = BuildAdjacencyList();
        var distances = new Dictionary<CityNode, (int distance, int cost)>();
        var previous = new Dictionary<CityNode, (CityNode city, Route route)?>();
        var unvisited = new HashSet<CityNode>(Cities);

        foreach (var city in Cities) distances[city] = (int.MaxValue, int.MaxValue);
        distances[start] = (0, 0);

        while (unvisited.Count > 0)
        {
            var current =
                unvisited.MinBy(c => distances[c].distance + distances[c].cost * 0.5)!; // Weight both distance and cost
            if (current == end) break;

            unvisited.Remove(current);

            foreach (var (neighbor, route) in adjList[current])
            {
                if (!unvisited.Contains(neighbor)) continue;

                var altDistance = distances[current].distance + route.Distance;
                var altCost = distances[current].cost + route.Cost;

                // Consider both distance and cost in the optimization
                if (altDistance + altCost * 0.5 < distances[neighbor].distance + distances[neighbor].cost * 0.5)
                {
                    distances[neighbor] = (altDistance, altCost);
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

            var (prevCity, route) = previous[currentCity]!.Value;
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
            var cycles = new List<List<Route>>();
            var seenEdges = new HashSet<string>();

            var cycleColours = new[]
            {
            "#FF0000", //red
            "#00FFFF", //cyan
            "#800080", //purple
            "#FFFF00", //yellow
            "#00FF00", //lime
            "#FFA500", //orange
            "#008000", //green
            "#EAC117"  //gold brown
        };

            foreach (var startCity in Cities)
            {
                var visited = new HashSet<CityNode>();
                var path = new List<Route>();
                var pathCities = new HashSet<CityNode>();

                void DFS(CityNode current, int depth)
                {
                    if (depth > 10 || cycles.Count >= 20) // Limit search depth and number of cycles
                        return;

                    visited.Add(current);
                    pathCities.Add(current);

                    foreach (var (neighbor, route) in adjList[current])
                    {
                        // Create unique edge identifier
                        var edgeId = $"{Math.Min(current.GetHashCode(), neighbor.GetHashCode())}_{Math.Max(current.GetHashCode(), neighbor.GetHashCode())}";

                        if (neighbor == startCity && path.Count >= 2)
                        {
                            // Found a cycle
                            var newCycle = new List<Route>(path) { route };
                            var cycleEdges = new HashSet<string>();
                            bool isUniqueCycle = true;

                            // Check if this cycle is unique
                            foreach (var cycleRoute in newCycle)
                            {
                                var routeId = $"{Math.Min(cycleRoute.Start.GetHashCode(), cycleRoute.Destination.GetHashCode())}_{Math.Max(cycleRoute.Start.GetHashCode(), cycleRoute.Destination.GetHashCode())}";
                                if (!cycleEdges.Add(routeId))
                                {
                                    isUniqueCycle = false;
                                    break;
                                }
                            }

                            if (isUniqueCycle && newCycle.Count >= 3)
                            {
                                cycles.Add(newCycle);
                            }
                        }
                        else if (!pathCities.Contains(neighbor))
                        {
                            path.Add(route);
                            DFS(neighbor, depth + 1);
                            path.RemoveAt(path.Count - 1);
                        }
                    }

                    pathCities.Remove(current);
                    visited.Remove(current);
                }

                if (cycles.Count < 20) // Limit total number of cycles
                {
                    DFS(startCity, 0);
                }
            }

            // Clear all highlight colours first
            foreach (var route in Routes)
            {
                route.HighlightedColours.Clear();
                route.IsHighlighted = false;
            }

            // Apply the colours for each cycle
            for (var i = 0; i < cycles.Count; i++)
            {
                var cycleColour = cycleColours[i % cycleColours.Length];
                foreach (var route in cycles[i])
                {
                    route.HighlightedColours.Add(cycleColour);
                    route.IsHighlighted = true;
                }
            }

            // Build status message showing the paths
            if (cycles.Any())
            {
                StatusMessage = $"Found {cycles.Count} cycles:";
            }
            else
            {
                StatusMessage = "No cycles found";
            }
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
            var minCost = int.MaxValue;

            foreach (var startCity in Cities)
            {
                var visited = new HashSet<CityNode> { startCity };
                var adjList = BuildAdjacencyList();

                void FindCycles(CityNode current, List<Route> currentPath, int currentDistance, int currentCost)
                {
                    foreach (var (neighbor, route) in adjList[current])
                        if (neighbor == startCity && currentPath.Count > 2)
                        {
                            var totalDistance = currentDistance + route.Distance;
                            var totalCost = currentCost + route.Cost;
                            if (totalDistance < minDistance) // You can modify this condition to consider cost
                            {
                                minDistance = totalDistance;
                                minCost = totalCost;
                                shortestCycle = new List<Route>(currentPath) { route };
                            }
                        }
                        else if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            currentPath.Add(route);
                            FindCycles(neighbor, currentPath, currentDistance + route.Distance,
                                currentCost + route.Cost);
                            currentPath.RemoveAt(currentPath.Count - 1);
                            visited.Remove(neighbor);
                        }
                }

                FindCycles(startCity, new List<Route>(), 0, 0);
            }

            if (shortestCycle.Any())
            {
                HighlightPath(shortestCycle);
                StatusMessage = $"Found shortest cycle with distance {minDistance} km and cost {minCost} SEK";
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
    [JsonPropertyName("Startpunkt")] public required string From { get; set; }

    [JsonPropertyName("Destination")] public required string To { get; set; }

    [JsonPropertyName("Avstånd")] public int Distance { get; set; }

    [JsonPropertyName("Kostnad")] public int Cost { get; set; }
}