using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;

namespace LabShortestRouteFinder.ViewModel
{
    public class RouteViewModel
    {
        public ObservableCollection<Route> Routes { get; }

        public RouteViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Routes collection
            Routes = mainViewModel.Routes;
        }
    }
}
