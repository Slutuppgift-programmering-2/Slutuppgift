using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LabShortestRouteFinder.ViewModel
{
    public partial class ListViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _statusMessage;

        public ObservableCollection<Route> Routes { get; }
        public ObservableCollection<CityNode> Cities { get; }
        private readonly MainViewModel _mainViewModel;

        public ListViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Routes = mainViewModel.Routes;
            Cities = mainViewModel.Cities;
        }

        public void SaveRoutesToFile()
        {
            try
            {
                if (Routes == null || !Routes.Any())
                {
                    StatusMessage = "No routes available to save";
                    return;
                }

                var routeInfos = Routes
                    .Where(r => 
                        r != null && 
                        r.Start != null && 
                        r.Destination != null && 
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

                if (!routeInfos.Any())
                {
                    StatusMessage = "No valid routes to save. Please ensure all routes have valid cities, distances, and costs greater than 0.";
                    return;
                }

                var directoryPath = @"..\net8.0-windows\Resources";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                string filePath = Path.Combine(directoryPath, "routes.json");
                string updatedJson = JsonSerializer.Serialize(routeInfos, options);
                File.WriteAllText(filePath, updatedJson);
                
                StatusMessage = $"Successfully saved {routeInfos.Count} routes to file";
            }
            catch (UnauthorizedAccessException)
            {
                StatusMessage = "Access denied. Unable to save the file. Please check your permissions.";
            }
            catch (DirectoryNotFoundException)
            {
                StatusMessage = "Could not find the Resources directory. Please ensure it exists.";
            }
            catch (IOException ex)
            {
                StatusMessage = $"Error accessing the file: {ex.Message}";
            }
            catch (JsonException)
            {
                StatusMessage = "Error creating JSON data. Please check the route data format.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Unexpected error while saving: {ex.Message}";
            }
        }
    }
}