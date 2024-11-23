using LabShortestRouteFinder;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using LabShortestRouteFinder.Services;

namespace LabShortestRouteFinder.ViewModel
{
    public class GraphViewModel
    {
        public ObservableCollection<CityNode> Cities { get; }
        public ObservableCollection<Route> Routes { get; }
        
        private ObservableCollection<List<CityNode>> cycles;
        private List<CityNode> shortestCycle;
        private CityNode selectedStartCity;
        private CityNode selectedEndCity;
        private PathFinderService.PathResult currentPath;
        
        public double ShortestCycleDistance => shortestCycle != null ? CalculateCycleDistance(shortestCycle) : 0;
        public double CurrentPathDistance => currentPath?.Distance ?? 0;
        private readonly PathFinderService pathFinder;

        public GraphViewModel(MainViewModel mainViewModel) 
        {
            Cities = mainViewModel.Cities;
            Routes = mainViewModel.Routes;
            cycles = new ObservableCollection<List<CityNode>>();
            pathFinder = new PathFinderService();
            Routes.CollectionChanged += (s, e) => FindCycles();
        }

        public CityNode SelectedStartCity
        {
            get => selectedStartCity;
            set
            {
                selectedStartCity = value;
                OnPropertyChanged();
                FindShortestPath();
            }
        }

        public CityNode SelectedEndCity
        {
            get => selectedEndCity;
            set
            {
                selectedEndCity = value;
                OnPropertyChanged();
                FindShortestPath();
            }
        }

        public PathFinderService.PathResult CurrentPath
        {
            get => currentPath;
            private set
            {
                currentPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentPathDistance));
            }
        }

        public ObservableCollection<List<CityNode>> Cycles
        {
            get => cycles;
            private set
            {
                cycles = value;
                OnPropertyChanged();
            }
        }

        public List<CityNode> ShortestCycle
        {
            get => shortestCycle;
            private set
            {
                shortestCycle = value;
                OnPropertyChanged();
            }
        }

        private void FindShortestPath()
        {
            ResetPathMarkers();
            
            if (selectedStartCity != null && selectedEndCity != null)
            {
                var result = pathFinder.FindShortestPath(Cities, Routes, selectedStartCity, selectedEndCity);
                CurrentPath = result;
                if (result.HasPath)
                {
                    MarkPath(result);
                }
            }
            else
            {
                CurrentPath = PathFinderService.PathResult.Empty;
            }
        }

        private void MarkPath(PathFinderService.PathResult path)
        {
            foreach (var route in path.Routes)
            {
                route.IsPartOfPath = true;
                route.Start.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfPath));
                route.Destination.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfPath));
            }
        }

        private void ResetPathMarkers()
        {
            foreach (var route in Routes)
            {
                if (route.IsPartOfPath)
                {
                    route.IsPartOfPath = false;
                    route.Start.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfPath));
                    route.Destination.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfPath));
                }
            }
        }

        private void FindCycles()
        {
            ResetCycleFlags();

            var _cycles = new ObservableCollection<List<CityNode>>();
            var visited = new HashSet<CityNode>();
            var recursionStack = new HashSet<CityNode>();
            var currentPath = new Stack<CityNode>();

            foreach (var city in Cities)
            {
                if (!visited.Contains(city))
                {
                    DFSForCycles(city, visited, recursionStack, currentPath, _cycles);
                }
            }
            
            Cycles = _cycles;
            FindShortestCycle();
            MarkCycles();
        }

        private void DFSForCycles(CityNode current, HashSet<CityNode> visited,
            HashSet<CityNode> recursionStack, Stack<CityNode> currentPath,
            ObservableCollection<List<CityNode>> cycles)
        {
            visited.Add(current);
            recursionStack.Add(current);
            currentPath.Push(current);

            var neighbours = Routes
                .Where(r => r.Start == current)
                .Select(r => r.Destination);

            foreach (var neighbour in neighbours)
            {
                if (!visited.Contains(neighbour))
                {
                    DFSForCycles(neighbour, visited, recursionStack,
                        currentPath, cycles);
                }
                else if (recursionStack.Contains(neighbour))
                {
                    var cycle = currentPath.Reverse().TakeWhile(c => c != neighbour).Reverse().ToList();
                    cycle.Add(neighbour);
                    cycles.Add(cycle);
                }
            }
            
            recursionStack.Remove(current);
            currentPath.Pop();
        }

        private void FindShortestCycle()
        {
            if (!Cycles.Any()) return;

            ShortestCycle = Cycles
                .OrderBy(cycle => CalculateCycleDistance(cycle))
                .First();
        }

        private double CalculateCycleDistance(List<CityNode> cycle)
        {
            double totalDistance = 0;
            for (int i = 0; i < cycle.Count; i++)
            {
                var start = cycle[i];
                var end = cycle[(i + 1) % cycle.Count];

                var route = Routes.FirstOrDefault(r =>
                    r.Start == start && r.Destination == end);

                if (route != null)
                {
                    totalDistance += route.Distance;
                }
            }
            return totalDistance;
        }

        private void ResetCycleFlags()
        {
            foreach (var route in Routes)
            {
                if (route.IsPartOfCycle)
                {
                    route.IsPartOfCycle = false;
                    route.Start.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfCycle));
                    route.Destination.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfCycle));
                }
            }
        }

        private void MarkCycles()
        {
            foreach (var cycle in Cycles)
            {
                for (int i = 0; i < cycle.Count; i++)
                {
                    var start = cycle[i];
                    var end = cycle[(i + 1) % cycle.Count];
                    var route = Routes.FirstOrDefault(r =>
                        r.Start == start && r.Destination == end);
                    if (route != null)
                    {
                        route.IsPartOfCycle = true;
                        route.Start.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfCycle));
                        route.Destination.NotifyRoutePropertyChanged(route, nameof(Route.IsPartOfCycle));
                    }
                }
            }
        }
    }
}