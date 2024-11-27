using LabShortestRouteFinder.Converters;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;

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

            try
            {
                // Load cities first, then routes
                LoadCitiesFromFile();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCitiesFromFile()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "cities.json");
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cities file not found at: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);

            // Deserialize the JSON content into a list of CityNode objects
            List<CityNode> cities = JsonSerializer.Deserialize<List<CityNode>>(jsonContent) ?? new List<CityNode>();

            double windowWidth = 800;
            double windowHeight = 600;

            // Min and Max for Sweden
            double minLatitude = 55.0;
            double maxLatitude = 69.0;
            double minLongitude = 11.0;
            double maxLongitude = 24.0;

            double scaleFactorX = windowWidth / (maxLongitude - minLongitude);
            double scaleFactorY = windowHeight / (maxLatitude - minLatitude);

            foreach (var c in cities)
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