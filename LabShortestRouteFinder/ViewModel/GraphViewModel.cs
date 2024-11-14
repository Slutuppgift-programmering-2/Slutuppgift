using LabShortestRouteFinder;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;

namespace LabShortestRouteFinder.ViewModel
{
    public class GraphViewModel
    {
        public ObservableCollection<CityNode> Cities { get; }
        public ObservableCollection<Route> Routes { get; }

        public GraphViewModel(MainViewModel mainViewModel) 
        {
            Cities = mainViewModel.Cities;
            Routes = mainViewModel.Routes;
        }
    }
}
