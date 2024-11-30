using LabShortestRouteFinder.Converters;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace LabShortestRouteFinder.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CityNode> Cities { get; private set; }
        public ObservableCollection<Route> Routes { get; private set; }
        public RectangleCoordinates Rectangle { get; private set; }

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

            // Deserialize the JSON content
            CitiesRoot data = JsonSerializer.Deserialize<CitiesRoot>(jsonContent);

            Rectangle = data.Rectangle;
            List<CityNode> cities = data.Cities;

            double windowWidth = 433;
            double windowHeight = 842;

            // Min and Max for Sweden
            double minLatitude = Rectangle.SouthWest.Latitude;
            double maxLatitude = Rectangle.NorthEast.Latitude;
            double minLongitude = Rectangle.SouthWest.Longitude;
            double maxLongitude = Rectangle.NorthEast.Longitude;

            double scaleFactorX = windowWidth / (maxLongitude - minLongitude);
            double scaleFactorY = windowHeight / (maxLatitude - minLatitude);

            foreach (var c in cities)
            {
                c.X = (int)((c.Longitude - minLongitude) * scaleFactorX);
                c.Y = (int)(windowHeight - ((c.Latitude - minLatitude) * scaleFactorY));
                Cities.Add(c);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}