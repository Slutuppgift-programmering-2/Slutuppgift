using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ShortestRouteFinder.Models;
using ShortestRouteFinder.Services;
using ShortestRouteFinder.ViewModel;
using ShortestRouteFinder.Controls;
using System.Diagnostics;

namespace ShortestRouteFinder
{
    public partial class App
    {
        private IServiceProvider? ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Load data
            var cities = LoadCitiesData();
            if (cities == null)
            {
                Shutdown();
                return;
            }

            // Register configuration
            services.AddSingleton(cities);

            // Register services
            services.AddSingleton<PathFinder>();

            // Register controls
            services.AddTransient<MapControl>();

            // Register view models
            services.AddTransient<MainViewModel>();

            // Register windows
            services.AddTransient<MainWindow>();
        }

        private List<City>? LoadCitiesData()
        {
            try
            {
                string jsonData = """
                [
                  {
                    "Name": "Paris",
                    "X": 0.48,
                    "Y": 0.42,
                    "Penalty": 0.3,
                    "ConnectedTo": [ "London", "Brussels", "Frankfurt", "Lyon" ]
                  },
                  // ... your JSON data here ...
                ]
                """;

                Debug.WriteLine("Loading cities data...");
                var cities = JsonSerializer.Deserialize<List<City>>(jsonData);
                Debug.WriteLine($"Loaded {cities?.Count ?? 0} cities");
                
                if (cities == null || cities.Count == 0)
                {
                    MessageBox.Show("No cities data was loaded.", "Data Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                return cities;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading cities: {ex}");
                MessageBox.Show($"Error loading cities data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}