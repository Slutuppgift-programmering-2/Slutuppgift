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

           

            // New method to read cities from a file
            LoadDataFromFile();
        }

        // New method to read cities from a json file. We also calculate X and Y so that they scale to the canvas size
        private void LoadDataFromFile()
        {
            string jsonContent = File.ReadAllText("..\\net8.0-windows\\Resources\\cities.json");

            // Deserialize the JSON content into a list of CityNode objects
            List<CityNode> cities = JsonSerializer.Deserialize<List<CityNode>>(jsonContent) ?? new List<CityNode>();

            double windowWidth = 433;
            double windowHeight = 842;

            // Min and Max for Sweden
            double minLatitude = 55.0;
            double maxLatitude = 69.0;
            double minLongitude = 11.0;
            double maxLongitude = 24.0;

            double scaleFactorX = windowWidth / (maxLongitude - minLongitude);
            double scaleFactorY = windowHeight / (maxLatitude - minLatitude);

            foreach(var c in cities)
            {
                c.X = (int)Math.Round((c.Longitude - minLongitude) * scaleFactorX, 0);
                c.Y = (int)Math.Round(windowHeight - ((c.Latitude - minLatitude) * scaleFactorY), 0);
                Cities.Add(c);
            }
        }

       

       

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
