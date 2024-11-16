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
                  {
                    "Name": "London",
                    "X": 0.45,
                    "Y": 0.35,
                    "Penalty": 0.4,
                    "ConnectedTo": [ "Paris", "Amsterdam", "Edinburgh" ]
                  },
                  {
                    "Name": "Amsterdam",
                    "X": 0.51,
                    "Y": 0.33,
                    "Penalty": 0.2,
                    "ConnectedTo": [ "London", "Brussels", "Berlin" ]
                  },
                  {
                    "Name": "Brussels",
                    "X": 0.50,
                    "Y": 0.37,
                    "Penalty": 0.15,
                    "ConnectedTo": [ "Paris", "Amsterdam", "Frankfurt" ]
                  },
                  {
                    "Name": "Berlin",
                    "X": 0.58,
                    "Y": 0.35,
                    "Penalty": 0.25,
                    "ConnectedTo": [ "Amsterdam", "Frankfurt", "Prague", "Warsaw" ]
                  },
                  {
                    "Name": "Frankfurt",
                    "X": 0.53,
                    "Y": 0.40,
                    "Penalty": 0.2,
                    "ConnectedTo": [ "Paris", "Brussels", "Berlin", "Munich" ]
                  },
                  {
                    "Name": "Munich",
                    "X": 0.55,
                    "Y": 0.45,
                    "Penalty": 0.2,
                    "ConnectedTo": [ "Frankfurt", "Prague", "Vienna", "Milan" ]
                  },
                  {
                    "Name": "Prague",
                    "X": 0.60,
                    "Y": 0.42,
                    "Penalty": 0.15,
                    "ConnectedTo": [ "Berlin", "Munich", "Vienna", "Warsaw" ]
                  },
                  {
                    "Name": "Vienna",
                    "X": 0.62,
                    "Y": 0.47,
                    "Penalty": 0.2,
                    "ConnectedTo": [ "Prague", "Munich", "Budapest", "Milan" ]
                  },
                  {
                    "Name": "Warsaw",
                    "X": 0.65,
                    "Y": 0.38,
                    "Penalty": 0.3,
                    "ConnectedTo": [ "Berlin", "Prague", "Budapest" ]
                  },
                  {
                    "Name": "Budapest",
                    "X": 0.65,
                    "Y": 0.48,
                    "Penalty": 0.25,
                    "ConnectedTo": [ "Vienna", "Warsaw", "Rome" ]
                  },
                  {
                    "Name": "Milan",
                    "X": 0.55,
                    "Y": 0.52,
                    "Penalty": 0.35,
                    "ConnectedTo": [ "Munich", "Vienna", "Rome", "Lyon" ]
                  },
                  {
                    "Name": "Rome",
                    "X": 0.58,
                    "Y": 0.58,
                    "Penalty": 0.4,
                    "ConnectedTo": [ "Milan", "Budapest", "Barcelona" ]
                  },
                  {
                    "Name": "Lyon",
                    "X": 0.50,
                    "Y": 0.48,
                    "Penalty": 0.2,
                    "ConnectedTo": [ "Paris", "Milan", "Barcelona" ]
                  },
                  {
                    "Name": "Barcelona",
                    "X": 0.45,
                    "Y": 0.55,
                    "Penalty": 0.3,
                    "ConnectedTo": [ "Lyon", "Rome", "Madrid" ]
                  },
                  {
                    "Name": "Madrid",
                    "X": 0.40,
                    "Y": 0.60,
                    "Penalty": 0.35,
                    "ConnectedTo": [ "Barcelona", "Lisbon" ]
                  },
                  {
                    "Name": "Lisbon",
                    "X": 0.35,
                    "Y": 0.62,
                    "Penalty": 0.25,
                    "ConnectedTo": [ "Madrid" ]
                  },
                  {
                    "Name": "Edinburgh",
                    "X": 0.43,
                    "Y": 0.28,
                    "Penalty": 0.3,
                    "ConnectedTo": [ "London" ]
                  }
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