using LabShortestRouteFinder.Converters;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;

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

            NormalizeCoordinates();
        }

        private void NormalizeCoordinates()
        {
            int maxX = Cities.Max(c => c.X);
            int maxY = Cities.Max(c => c.Y);

            foreach (var city in Cities)
            {
                city.X = (city.X * 800) / maxX; // Normalize to Canvas width
                city.Y = (city.Y * 600) / maxY; // Normalize to Canvas height
            }
        }

        public void LoadData()
        {
            // For simplicity, add sample data here or load from a JSON file
            var cityA = new CityNode { Name = "Stockholm", Latitude = 59.3293, Longitude = 18.0686 };
            var cityB = new CityNode { Name = "Göteborg", Latitude = 57.7089, Longitude = 11.9746 };
            var cityC = new CityNode { Name = "Malmö", Latitude = 55.6050, Longitude = 13.0038 };
            Cities.Add(cityA);
            Cities.Add(cityB);
            Cities.Add(cityC);

            // Definiera koordinaterna för kartområdet (t.ex. Sverige)
            double minLat = 55.0;
            double maxLat = 69.0;
            double minLon = 11.0;
            double maxLon = 24.0;
            // Fönstrets bredd och höjd (i pixlar)
            int windowWidth = 800;
            int windowHeight = 600;
            MapWin mapWin = new MapWin(minLat, maxLat, minLon, maxLon, windowWidth, windowHeight);

            var transform = new MapTransformer(mapWin);
            var transformedCities = transform.TransformCities(Cities);
            Routes.Add(new Route { Start = transformedCities[0], Destination = transformedCities[1], Distance = 474 });
            Routes.Add(new Route { Start = transformedCities[0], Destination = transformedCities[2], Distance = 617 });
            Routes.Add(new Route { Start = transformedCities[1], Destination = transformedCities[2], Distance = 259 });
            Routes.Add(new Route { Start = cityA, Destination = cityB, Distance = 474 });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
