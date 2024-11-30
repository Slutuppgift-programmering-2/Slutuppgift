using LabShortestRouteFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;

namespace LabShortestRouteFinder.ViewModel
{
    public class CityViewModel
    {
        public ObservableCollection<CityNode> Cities { get; }

        private RectangleCoordinates Rectangle { get; }

        public CityNode SelectedCity { get; set; }

        public CityViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Cities collection
            Cities = mainViewModel.Cities;
            Rectangle = mainViewModel.Rectangle;
        }

        public void SaveCitiesToFile()
        {
            double windowWidth = 433;
            double windowHeight = 842;

            // Min and Max for Sweden
            double minLatitude = Rectangle.SouthWest.Latitude;
            double maxLatitude = Rectangle.NorthEast.Latitude;
            double minLongitude = Rectangle.SouthWest.Longitude;
            double maxLongitude = Rectangle.NorthEast.Longitude;

            double scaleFactorX = windowWidth / (maxLongitude - minLongitude);
            double scaleFactorY = windowHeight / (maxLatitude - minLatitude);

            foreach (var c in Cities)
            {
                if (string.IsNullOrEmpty(c.Name))

                {

                    MessageBox.Show("Namn på stad får ej vara tomt ");

                    return;

                }

                if (c.Latitude < minLatitude || c.Latitude > maxLatitude || c.Longitude < minLongitude || c.Longitude > maxLongitude)

                {

                    MessageBox.Show("Felaktiga koordinater för " + c.Name);

                    return;

                }

                c.X = (int)((c.Longitude - minLongitude) * scaleFactorX);
                c.Y = (int)(windowHeight - ((c.Latitude - minLatitude) * scaleFactorY));
            }

            // Fix that å ö ä is saved correctly
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            CitiesRoot citiesRoot = new CitiesRoot();
            citiesRoot.Rectangle = Rectangle;
            citiesRoot.Cities = Cities.ToList();

            string updatedJson = JsonSerializer.Serialize(citiesRoot, options);

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "cities.json");

            File.WriteAllText(filePath, updatedJson);

            filePath = "..\\..\\..\\Resources\\cities.json";

            File.WriteAllText(filePath, updatedJson);


        }
    }
}
