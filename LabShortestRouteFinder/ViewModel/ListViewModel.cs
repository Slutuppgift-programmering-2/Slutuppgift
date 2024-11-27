using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace LabShortestRouteFinder.ViewModel
{
    public class ListViewModel
    {
        public ObservableCollection<Route> Routes { get; }

        public ListViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Routes collection
            Routes = mainViewModel.Routes;
        }

        public void SaveRoutesToFile()
        {
            try
            {
                // Validate and prepare the routes for saving
                var routeInfos = Routes
                    .Where(r =>
                        r.Distance > 0 &&
                        r.Cost > 0)
                    .Select(r => new RouteInfo
                    {
                        From = r.Start.Name,
                        To = r.Destination.Name,
                        Distance = r.Distance,
                        Cost = r.Cost
                    })
                    .ToList();

                // Configure JSON serialization options
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                // Serialize and save to file
                string updatedJson = JsonSerializer.Serialize(routeInfos, options);
                File.WriteAllText(@"..\net8.0-windows\Resources\routes.json", updatedJson);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during saving
                System.Diagnostics.Debug.WriteLine($"Error saving routes: {ex.Message}");
            }
        }
    }
}