using LabShortestRouteFinder.Converters;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace LabShortestRouteFinder.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CityNode> Cities { get; private set; }
        public ObservableCollection<Route> Routes { get; private set; }

        public MainViewModel()
        {
            // Initialize collections
            Cities = new ObservableCollection<CityNode>();
            Routes = new ObservableCollection<Route>();

            // Load data and normalize
            //LoadData();
            LoadDataFromFile();
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

        private void LoadDataFromFile()
        {
            string jsonContent = File.ReadAllText("..\\net8.0-windows\\Resources\\cities.json");

            // Deserialize the JSON content into a list of CityNode objects
            List<CityNode> cities = JsonSerializer.Deserialize<List<CityNode>>(jsonContent) ?? new List<CityNode>();

            foreach(var c in cities)
            {
                Cities.Add(c);
            }

            // Transform city coordinates
            double minLat = 59.0;
            double maxLat = 65.5;
            double minLon = 13.0;
            double maxLon = 20.5;
            int windowWidth = 800;
            int windowHeight = 600;



            MapWin mapWin = new MapWin(minLat, maxLat, minLon, maxLon, windowWidth, windowHeight);
            var transformer = new MapTransformer(mapWin);
            var transformedCities = transformer.TransformCities(Cities);

            // Add routes
            //Routes.Add(new Route { Start = transformedCities[0], Destination = transformedCities[1], Distance = 474 });
            //Routes.Add(new Route { Start = transformedCities[0], Destination = transformedCities[2], Distance = 617 });
            //Routes.Add(new Route { Start = transformedCities[1], Destination = transformedCities[2], Distance = 259 });
        }

        private void LoadData()
        {
            // Add sample data
            var cityA = new CityNode { Name = "Stockholm", Latitude = 59.3293, Longitude = 18.0686 };
            var cityB = new CityNode { Name = "Göteborg", Latitude = 57.7089, Longitude = 11.9746 };
            var cityC = new CityNode { Name = "Malmö", Latitude = 55.6050, Longitude = 13.0038 };
            Cities.Add(cityA);
            Cities.Add(cityB);
            Cities.Add(cityC);

            // Transform city coordinates
            double minLat = 55.0;
            double maxLat = 69.0;
            double minLon = 11.0;
            double maxLon = 24.0;
            int windowWidth = 800;
            int windowHeight = 600;

            MapWin mapWin = new MapWin(minLat, maxLat, minLon, maxLon, windowWidth, windowHeight);
            var transformer = new MapTransformer(mapWin);
            var transformedCities = transformer.TransformCities(Cities);

            // Add routes
            Routes.Add(new Route { Start = transformedCities[0], Destination = transformedCities[1], Distance = 474 });
            Routes.Add(new Route { Start = transformedCities[0], Destination = transformedCities[2], Distance = 617 });
            Routes.Add(new Route { Start = transformedCities[1], Destination = transformedCities[2], Distance = 259 });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
