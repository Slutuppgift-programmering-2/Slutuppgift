using LabShortestRouteFinder;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;

namespace LabShortestRouteFinder.ViewModel
{
    public class GraphViewModel
    {
        public ObservableCollection<CityNode> Cities { get; }
        public ObservableCollection<Route> Routes { get; }
        
        private ObservableCollection<List<CityNode>> cycles;

        public GraphViewModel(MainViewModel mainViewModel) 
        {
            Cities = mainViewModel.Cities;
            Routes = mainViewModel.Routes;
            cycles = new ObservableCollection<List<CityNode>>();
            
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

        private void FindCycles()
        {
            ResetCycleFlags();

            var _cycles = new ObservableCollection<List<CityNode>>();
            var visited = new HashSet<CityNode>();
            var recursionStack = new HashSet<CityNode>(); //Sparar noder i aktuell väg
            var currentPath = new Stack<CityNode>();

            foreach (var city in Cities)
            {
                if (!visited.Contains(city))
                {
                    DFSForCycles(city, visited, recursionStack, currentPath, _cycles);
                }
            }
            
            Cycles = _cycles;
            MarkCycles();

        }

        // Depth-First Search 
        private void DFSForCycles(CityNode current, HashSet<CityNode> visited,
            HashSet<CityNode> recursionStack, Stack<CityNode> currentPath,
            ObservableCollection<List<CityNode>> cycles)
        {
            visited.Add(current);
            recursionStack.Add(current);
            currentPath.Push(current);

            //Hämtar Nodens grannar
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
                else if (recursionStack.Contains(neighbour)) //Cykel hittad
                {
                    var cycleStart = currentPath.Reverse().TakeWhile(c => c != neighbour).Reverse().ToList(); 
                    cycle.Add(neighbour);
                    cycles.Add(cycle);
                }
            }
            
            recursionStack.Remove(current);
            currentPath.Pop();
        }

        private void ResetCycleFlags()
        {
            foreach (var city in Cities)
            {
                city.IsPartOfCycle = false;
            }

            foreach (var route in Routes)
            {
                route.IsPartOfCycle = false;
            }
        }

        // markerar noder och vägar som hör med hittad cykel
        private void MarkCycles()
        {
            foreach (var cycle in Cycles)
            {
                foreach (var city in cyclee)
                {
                    city.IsPartOfCycle = true;
                }

                for (int i = 0; i < cycle.Count; i++)
                {
                    var start = cycle[i];
                    var end = cycle[(i + 1) % cycle.Count];
                    var route = Routes.FirstOrDefault(r =>
                        r.Start == start && r.Destination == end);
                    if (route != null)
                    {
                        route.IsPartOfCycle = true;
                    }
                }
            }
        }
    }
}
