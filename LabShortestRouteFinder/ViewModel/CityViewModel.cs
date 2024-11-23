using LabShortestRouteFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LabShortestRouteFinder.ViewModel
{
    public class CityViewModel
    {
        public ObservableCollection<CityNode> Cities { get; }

        public CityViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Cities collection
            Cities = mainViewModel.Cities;
        }

        public void SaveCitiesToFile()
        {
            double windowWidth = 800;
            double windowHeight = 600;

            // Min and Max for Sweden
            double minLatitude = 55.0;
            double maxLatitude = 69.0;
            double minLongitude = 11.0;
            double maxLongitude = 24.0;

            double scaleFactorX = windowWidth / (maxLongitude - minLongitude);
            double scaleFactorY = windowHeight / (maxLatitude - minLatitude);

            foreach (var c in Cities)
            {
                //Validate that the X Y is within the allowed bounds
                if (c.X < 0 || c.Y < 0 || c.X > 800 || c.Y > 600)
                    continue;

                // Check that name is not empty
                if (string.IsNullOrEmpty(c.Name))
                    continue;

                c.Longitude = (c.X / scaleFactorX) + minLongitude;
                c.Latitude = minLatitude + (windowHeight - c.Y) / scaleFactorY;
            }

            // Fix that å ö ä is saved correctly
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            string updatedJson = JsonSerializer.Serialize(Cities, options);

            File.WriteAllText("..\\net8.0-windows\\Resources\\cities.json", updatedJson);

        }
    }
}
