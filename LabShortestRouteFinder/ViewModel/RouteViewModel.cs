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

        // Add any additional methods for route manipulation or retrieval
        public ObservableCollection<Route> GetRoutesForCity(CityNode city)
        {
            // Return routes where the city is either the start or destination
            var relatedRoutes = new ObservableCollection<Route>();
            foreach (var route in Routes)
            {
                if (route.Start == city || route.Destination == city)
                {
                    relatedRoutes.Add(route);
                }
            }
            return relatedRoutes;
        }
    }
}

