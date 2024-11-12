using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LabShortestRouteFinder.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CityNode> Cities { get; private set; }
        public ObservableCollection<Route> Routes { get; private set; }

        public MainViewModel()
        {
            // Initialize data here or load from JSON
            Cities = new ObservableCollection<CityNode>();
            Routes = new ObservableCollection<Route>();

            LoadData();

            // Listen for changes in the Routes collection
//            Routes.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Routes));
        }

        public void LoadData()
        {
            // For simplicity, add sample data here or load from a JSON file
            var cityA = new CityNode { Name = "CityA", X = 100, Y = 100 };
            var cityB = new CityNode { Name = "CityB", X = 300, Y = 200 };
            Cities.Add(cityA);
            Cities.Add(cityB);
            Routes.Add(new Route { Start = cityA, Destination = cityB, Distance = 150 });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
